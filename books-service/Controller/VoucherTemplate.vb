Imports System.Collections.ObjectModel
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports utility_service

Namespace Controller
    Public Class VoucherTemplate

        Public Shared Function SaveVoucherTemplate(voucherTemplate As Model.VoucherTemplate, conn As MySqlConnection)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If voucherTemplate.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`voucher_template` (supplier_id,supplier_account_id,voucher_json)values(@supplier_id,@supplier_account_id,@voucher_json)"
                ElseIf voucherTemplate.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`voucher_template` where id=@id;"
                    command.Parameters.AddWithValue("id", voucherTemplate.Id)
                ElseIf voucherTemplate.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`voucher_template` (id,supplier_id,supplier_account_id,voucher_json)values(@id,@supplier_id,@supplier_account_id,@voucher_json)"
                    command.Parameters.AddWithValue("id", voucherTemplate.Id)
                End If

                If voucherTemplate.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("supplier_id", voucherTemplate.Supplier_Id)
                    command.Parameters.AddWithValue("supplier_account_id", voucherTemplate.Supplier_Account_Id)

                    voucherTemplate.Voucher.Id = 0
                    Dim settings As New JsonSerializerSettings
                    settings.NullValueHandling = NullValueHandling.Ignore
                    settings.DefaultValueHandling = DefaultValueHandling.Ignore
                    command.Parameters.AddWithValue("voucher_json", JsonConvert.SerializeObject(voucherTemplate.Voucher, Formatting.Indented, settings))
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveVoucherTemplate", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return Nothing
        End Function

        Public Shared Function LoadVoucherTemplates(databaseManager As Manager.Mysql) As ObservableCollection(Of Model.VoucherTemplate)
            Dim voucherTemplates = New ObservableCollection(Of Model.VoucherTemplate)

            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.voucher_template;")
                    If reader.HasRows Then
                        While reader.Read
                            voucherTemplates.Add(New Model.VoucherTemplate(reader))
                        End While
                    End If
                End Using

            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadVoucherTemplates", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return voucherTemplates
        End Function

        Public Shared Function GetTemplate(databaseManager As Manager.Mysql, supplier_id As Integer, supplier_account_id As Integer) As Model.VoucherTemplate
            Dim template As Model.VoucherTemplate = Nothing

            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("SELECT * FROM `voucher_entry`.voucher_template WHERE supplier_id={0} OR (supplier_id={0} AND supplier_account_id={1}) ORDER BY `timestamp` DESC LIMIT 1;", supplier_id, supplier_account_id))
                    If reader.HasRows Then
                        reader.Read()
                        template = New Model.VoucherTemplate(reader)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadVoucherTemplates", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return template
        End Function
    End Class
End Namespace