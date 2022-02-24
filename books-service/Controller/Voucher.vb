Imports System.Collections.ObjectModel
Imports System.Drawing
Imports System.IO
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports NPOI.HSSF.UserModel
Imports utility_service

Namespace Controller

    Public Class Voucher

        Public Shared Sub Print(voucher As Model.Voucher, startupPath As String)
            Dim templatePath As String = startupPath & "\voucher template.xls"
            Dim voucherDirectory As String = startupPath & "\vouchers\"
            Directory.CreateDirectory(voucherDirectory)
            Dim newVoucherName As String = voucherDirectory & Now.ToString("yyyyMMddHHmm") & ".xls"
            File.Copy(templatePath, newVoucherName, True)

            Dim book As New HSSFWorkbook

            Using fs As New FileStream(newVoucherName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)
                book = New HSSFWorkbook(fs)
            End Using

            '  Const secondPageStartIndex As Integer = 38
            Dim sheet As HSSFSheet = book.GetSheetAt(0)
            Dim row As HSSFRow = sheet.GetRow(0)
            Dim cell As HSSFCell = row.Cells(0)


            For Each secondPageStartIndex As Integer In {0, 38}
                With voucher
                    sheet.GetRow(0 + secondPageStartIndex).Cells(0).SetCellValue(.Company_Name)
                    sheet.GetRow(2 + secondPageStartIndex).Cells(1).SetCellValue(String.Format("** {0} **", .Supplier_Payee))
                    sheet.GetRow(3 + secondPageStartIndex).Cells(8).SetCellValue(.Id.ToString("D5"))
                    sheet.GetRow(4 + secondPageStartIndex).Cells(1).SetCellValue(.Entry_Date.ToString("MMM dd, yyyy"))

                    sheet.GetRow(24 + secondPageStartIndex).Cells(7).SetCellValue(.Net_Amount_Words)
                    sheet.GetRow(28 + secondPageStartIndex).Cells(7).SetCellValue(.Bank_Account_Code)
                    sheet.GetRow(28 + secondPageStartIndex).Cells(9).SetCellValue(.Voucher_No)
                    sheet.GetRow(31 + secondPageStartIndex).Cells(6).SetCellValue("Received_By") '.Received_By)

                    sheet.GetRow(34 + secondPageStartIndex).Cells(0).SetCellValue("Prepared_By_Fullname") '.Prepared_By_Fullname)
                    sheet.GetRow(34 + secondPageStartIndex).Cells(3).SetCellValue("Certified_By_Fullname") '.Certified_By_Fullname)
                    sheet.GetRow(34 + secondPageStartIndex).Cells(6).SetCellValue("Approved_By_Fullname") '.Approved_By_Fullname)

                    Dim particularsSummary As String = "Representing Payment for: "
                    For Each d As Model.JournalAccountDistributionItem In .Journal_Account_Distributions
                        particularsSummary &= d.Journal_Account.Name & ", "
                    Next
                    particularsSummary = particularsSummary.TrimEnd()
                    particularsSummary = particularsSummary.TrimEnd(",")

                    sheet.GetRow(7 + secondPageStartIndex).Cells(1).SetCellValue(particularsSummary)

                    Dim i As Integer = 9
                    For Each d As Model.JournalAccountDistributionItem In .Journal_Account_Distributions
                        sheet.GetRow(i + secondPageStartIndex).Cells(1).SetCellValue(d.Journal_Account.Name)
                        sheet.GetRow(i + secondPageStartIndex).Cells(9).SetCellValue("PHP")
                        sheet.GetRow(i + secondPageStartIndex).Cells(10).SetCellValue(d.Amount)
                        If d.TAX_Amount > 0 Then
                            i += 1
                            sheet.GetRow(i + secondPageStartIndex).Cells(7).SetCellValue(String.Format("less {0}% wtax", d.W_TAX_Percentage))
                            sheet.GetRow(i + secondPageStartIndex).Cells(10).SetCellValue(d.TAX_Amount)
                        End If
                        i += 1
                    Next

                    sheet.GetRow(18 + secondPageStartIndex).Cells(9).SetCellValue("PHP")
                    sheet.GetRow(18 + secondPageStartIndex).Cells(10).SetCellValue(.Net_Amount)

                    i += .Journal_Account_Distributions.Count
                    For Each p As Model.ParticularsItem In .Particulars
                        sheet.GetRow(i + secondPageStartIndex).Cells(1).SetCellValue(String.Format("{0}:  {1}", p.Field, p.Value))
                        i += 1
                    Next

                    i = 25
                    For Each d As Model.JournalAccountDistributionItem In .Journal_Account_Distributions
                        'accounts
                        sheet.GetRow(i + secondPageStartIndex).Cells(0).SetCellValue(d.Journal_Account.Name)
                        sheet.GetRow(i + secondPageStartIndex).Cells(4).SetCellValue(d.Amount)
                        i += 1
                    Next

                    'wtax
                    If .Tax_Amount > 0 Then
                        sheet.GetRow(i + secondPageStartIndex).Cells(0).SetCellValue("        wtax")
                        sheet.GetRow(i + secondPageStartIndex).Cells(5).SetCellValue(.Tax_Amount)
                        i += 1
                    End If

                    'bank account
                    sheet.GetRow(i + secondPageStartIndex).Cells(0).SetCellValue(String.Format("        Cash in Bank({0})", .Bank_Account_Code))
                    sheet.GetRow(i + secondPageStartIndex).Cells(5).SetCellValue(.Net_Amount)
                End With
            Next

            Using fs As New FileStream(newVoucherName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)
                book.Write(fs)
            End Using

            book.Close()

            Dim oProcess As New Process
            oProcess.StartInfo.Verb = "Print"
            oProcess.StartInfo.FileName = newVoucherName
            oProcess.Start()
        End Sub

        Public Shared Function SaveVoucher(voucher As Model.Voucher, conn As MySqlConnection, user As utility_service.Model.User)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If voucher.ChangeState = States.ChangeState.Added Then
                    query = "insert into voucher_entry.voucher (voucher_no,entry_date, company_id, supplier_id, supplier_account_id, particulars, remarks, journal_account_distributions, bank_account_id)values" &
                                         "(voucher_entry.voucher_no(@company_id),@entry_date, @company_id, @supplier_id, @supplier_account_id, @particulars, @remarks, @journal_account_distributions, @bank_account_id)"
                ElseIf voucher.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`voucher` where id=@id;"
                    command.Parameters.AddWithValue("id", voucher.Id)
                ElseIf voucher.ChangeState = States.ChangeState.None Then
                    'query = "replace into `voucher` (id,entry_date, company_id, supplier_id, supplier_account_id, particulars, remarks, journal_account_distributions, bank_account_id)values (@id, @entry_date, @company_id, @supplier_id, @supplier_account_id, @particulars, @remarks, @journal_account_distributions, @bank_account_id)"

                    query = "update `voucher_entry`.`voucher` set entry_date=@entry_date, company_id=@company_id, supplier_id=@supplier_id, supplier_account_id=@supplier_account_id, particulars=@particulars, remarks=@remarks, journal_account_distributions=@journal_account_distributions, bank_account_id=@bank_account_id where id=@id;"
                    command.Parameters.AddWithValue("id", voucher.Id)
                End If

                If voucher.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("entry_date", voucher.Entry_Date)
                    command.Parameters.AddWithValue("company_id", voucher.Company.Id)
                    command.Parameters.AddWithValue("supplier_id", voucher.Supplier.Id)
                    command.Parameters.AddWithValue("supplier_account_id", voucher.Supplier_Account.Id)
                    command.Parameters.AddWithValue("particulars", JsonConvert.SerializeObject(voucher.Particulars, Formatting.Indented))
                    command.Parameters.AddWithValue("journal_account_distributions", JsonConvert.SerializeObject(voucher.Journal_Account_Distributions, Formatting.Indented))
                    command.Parameters.AddWithValue("bank_account_id", voucher.Bank_Account.Id)
                    command.Parameters.AddWithValue("remarks", voucher.Remarks)
                    command.Parameters.AddWithValue("certified_by_id", user.Id)
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveVoucher", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return Nothing
        End Function

        Public Shared Sub CompleteVoucherDetail(voucher As Model.Voucher, databaseManager As utility_service.Manager.Mysql)
            Dim ConnectionIsOpenedFromOutside As Boolean = databaseManager.Connection.State = ConnectionState.Open
            If Not ConnectionIsOpenedFromOutside Then databaseManager.Connection.Open()

            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("select * from voucher_entry.company where id={0}", voucher.Company_Id))
                    If reader.HasRows Then
                        reader.Read()
                        voucher.Company = New Model.Company(reader)
                    End If
                End Using

                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("select * from voucher_entry.company_bank_account_complete where id={0}", voucher.Bank_Account_Id))
                    If reader.HasRows Then
                        reader.Read()
                        voucher.Bank_Account = New Model.CompanyBankAccount(reader)
                    End If
                End Using

                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("select * from voucher_entry.supplier where id={0}", voucher.Supplier_Id))
                    If reader.HasRows Then
                        reader.Read()
                        voucher.Supplier = New Model.Supplier(reader)
                    End If
                End Using

                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("select * from voucher_entry.supplier_account_complete where id={0}", voucher.Supplier_Account_Id))
                    If reader.HasRows Then
                        reader.Read()
                        voucher.Supplier_Account = New Model.SupplierAccount(reader)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "CompleteVoucherDetail", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            If Not ConnectionIsOpenedFromOutside Then databaseManager.Connection.Close()
        End Sub

        Public Shared Sub ChangePrintStatus(voucher As Model.Voucher, _print_status As Model.Voucher.PrintStatusChoices, conn As MySqlConnection)
            Try
                Dim command As New MySqlCommand()
                Dim query As String = "update `voucher_entry`.`voucher` set print_status=@print_status where id=@id;"
                command.Parameters.AddWithValue("print_status", _print_status)
                command.Parameters.AddWithValue("id", voucher.Id)

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "ChangePrintStatus", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub

        Public Shared Function LoadVouchers(databaseManager As Manager.Mysql, Optional searchString As String = "", Optional companyName As String = "", Optional supplierPayee As String = "", Optional completeDetail As Boolean = False) As ObservableCollection(Of Model.Voucher)
            Dim query As String = "SELECT * FROM `voucher_entry`.voucher_complete;"
            If searchString <> "" Or companyName <> "" Or supplierPayee <> "" Then
                Dim conjunction As String = ""
                If searchString <> "" Then
                    query = String.Format("SELECT * FROM `voucher_entry`.voucher_complete where voucher_no like '%{0}%' or company_name like '{0}' or supplier_payee like '{0}' or supplier_account_number like '%{0}%' or bank_account_code like '%{0}%'", searchString) : conjunction = "and"
                Else
                    query = String.Format("SELECT * FROM `voucher_entry`.voucher_complete where", searchString)
                End If

                If companyName <> "" Then
                    query &= String.Format(" {0} company_name like '%{1}%'", conjunction, companyName) : conjunction = "and"
                End If
                If supplierPayee <> "" Then
                    query &= String.Format(" {0} supplier_payee like '%{1}%'", conjunction, supplierPayee)
                End If
            End If

            Dim Vouchers As New ObservableCollection(Of Model.Voucher)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(query)
                    If reader.HasRows Then
                        While reader.Read
                            Dim _voucher As New Model.Voucher(reader)
                            If completeDetail Then CompleteVoucherDetail(_voucher, databaseManager)
                            Vouchers.Add(_voucher)
                        End While
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "ChangePrintStatus", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return Vouchers
        End Function
    End Class
End Namespace
