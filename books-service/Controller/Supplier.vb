Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows
Imports MySql.Data.MySqlClient
Imports utility_service

Namespace Controller
    Public Class Supplier
        Public Shared Function GetSupplier(payee As String, tin As String, databaseManager As Manager.Mysql) As Model.Supplier
            Dim supplier As Model.Supplier = Nothing
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("SELECT * FROM `voucher_entry`.supplier WHERE payee='{0}' AND tin='{1}';", payee, tin))
                    If reader.HasRows Then
                        reader.Read()
                        supplier = New Model.Supplier(reader)
                        If reader.Read Then 'means there's more
                            Throw New Exception("Duplicate Supplier's TIN")
                        End If
                    End If
                End Using

            Catch ex As Exception
                MessageBox.Show(ex.Message, "GetSupplier", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return supplier
        End Function

        Public Shared Function SaveSupplier(supplier As Model.Supplier, conn As MySqlConnection)
            Try
                Dim query As String = ""
                Dim command As New MySqlCommand()
                If supplier.ChangeState = States.ChangeState.Added Then
                    query = "insert into `voucher_entry`.`supplier` (name,payee,address,tin,remarks)values(@name,@payee,@address,@tin,@remarks)"
                ElseIf supplier.ChangeState = States.ChangeState.Deleted Then
                    query = "delete from `voucher_entry`.`supplier` where id=@id;"
                    command.Parameters.AddWithValue("id", supplier.Id)
                ElseIf supplier.ChangeState = States.ChangeState.None Then
                    query = "replace into `voucher_entry`.`supplier` (id,payee,name,address,tin,remarks)values(@id,@payee,@name,@address,@tin,@remarks)"
                    command.Parameters.AddWithValue("id", supplier.Id)
                End If

                If supplier.ChangeState <> States.ChangeState.Deleted Then
                    command.Parameters.AddWithValue("name", supplier.Name)
                    command.Parameters.AddWithValue("payee", supplier.Payee)
                    command.Parameters.AddWithValue("address", supplier.Address)
                    command.Parameters.AddWithValue("tin", supplier.TIN)
                    command.Parameters.AddWithValue("remarks", supplier.Remarks)
                End If

                command.CommandText = query
                command.Connection = conn
                command.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "SaveSupplier", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return Nothing
        End Function

        Public Shared Function LoadSuppliers(databaseManager As Manager.Mysql, Optional searchString As String = "", Optional supplierName As String = "") As ObservableCollection(Of Model.Supplier)
            Dim query As String = "SELECT * FROM `voucher_entry`.supplier;"
            If searchString <> "" Or supplierName <> "" Then
                Dim conjunction As String = ""
                If searchString <> "" Then
                    query = String.Format("SELECT * FROM `voucher_entry`.supplier where tin like '%{0}%' or name like '%{0}%'", searchString) : conjunction = "and"
                Else
                    query = String.Format("SELECT * FROM `voucher_entry`.supplier where", searchString)
                End If

                If supplierName <> "" Then
                    query &= String.Format(" {0} name like '%{1}%'", conjunction, supplierName)
                End If
            End If

            Dim suppliers As New ObservableCollection(Of Model.Supplier)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(query)
                    If reader.HasRows Then
                        While reader.Read
                            suppliers.Add(New Model.Supplier(reader))
                        End While
                    Else : MessageBox.Show("There is no available Supplier to select.", "", MessageBoxButton.OK, MessageBoxImage.Error)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadSuppliers", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try

            Return suppliers
        End Function

        ''' <summary>
        ''' Group Suppliers By Payee.
        ''' </summary>
        ''' <param name="databaseManager"></param>
        ''' <returns></returns>
        Public Shared Function GroupSuppliers(databaseManager As Manager.Mysql) As ObservableCollection(Of Model.Supplier)
            Dim suppliers As New ObservableCollection(Of Model.Supplier)
            Try
                Using reader As MySqlDataReader = databaseManager.ExecuteDataReader("SELECT * FROM `voucher_entry`.supplier group by payee;")
                    If reader.HasRows Then
                        While reader.Read
                            suppliers.Add(New Model.Supplier(reader))
                        End While
                    Else : MessageBox.Show("There is no available Supplier to select, please finish setting up.", "", MessageBoxButton.OK, MessageBoxImage.Error)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.Message, "LoadSuppliers", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
            Return suppliers
        End Function


        Public Enum GroupByChoices
            PAYEE
            NAME
        End Enum


        Public Shared Sub ImportSuppliers(databaseManager As Manager.Mysql, filenames As String())
            For Each fl As String In filenames
                Using reader As New StreamReader(fl)
                    Dim l As String = reader.ReadLine : l = reader.ReadLine
                    While l IsNot Nothing
                        Dim args As String() = l.Split(";")
                        '0remarks,1supplier name, 2tin, 3address, 4city, 5zip, 6vat, 7sole proprietorship, 8proprietorship, 9contact number
                        Dim newSupplier As New Model.Supplier
                        With newSupplier
                            .Name = args(1).Trim.Trim.Trim.Trim.ToUpper
                            If .Name = "" Then Continue For
                            Try
                                .Payee = .Name

                                .Remarks = args(0).Trim
                                If args(9).Trim <> "" Then .Remarks &= ", Contact Address: " & args(9).Trim
                                If args(7).Trim <> "" Then .Remarks &= ", Proprietorship: " & args(7).Trim
                                If args(8).Trim <> "" Then .Remarks &= ", " & args(8).Trim
                                .Remarks = .Remarks.Trim(",").Trim()

                                Dim tinRegex As New Regex("([0-9-])+")
                                .TIN = tinRegex.Match(New Regex("[- ]").Replace(args(2), "")).Groups(0).Value

                                .Address = args(3).Trim
                                If args(4).Trim <> "" Then .Address &= ", " & args(4).Trim
                                If args(5).Trim <> "" Then .Address &= ", " & args(5).Trim
                            Catch ex As Exception
                                MessageBox.Show(ex.Message, "ImportSuppliers", MessageBoxButton.OK, MessageBoxImage.Error)
                            End Try
                            SaveSupplier(newSupplier, databaseManager.Connection)
                        End With
                        l = reader.ReadLine
                    End While

                End Using
            Next
        End Sub

    End Class


End Namespace
