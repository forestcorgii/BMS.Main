Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Model
    Public Class SupplierAccount
        Public Supplier As Supplier

        Public Property Id As Integer = 0 '0 supplier account id for irregular supplier

        Public Property Supplier_Id As Integer
        Public Property Account_Number As String

        Public Property ChangeState As States.ChangeState
        Sub New()

        End Sub
        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Supplier_Id = reader.Item("supplier_id")

            Account_Number = reader.Item("account_number")
            ChangeState = States.ChangeState.None
        End Sub

        Public Overrides Function ToString() As String
            Return Account_Number
        End Function
    End Class

End Namespace
