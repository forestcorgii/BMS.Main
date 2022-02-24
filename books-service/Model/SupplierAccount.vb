Imports System.Collections.ObjectModel
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Model
    Public Class SupplierAccount
        Public Supplier As Supplier
        Public Company As Company
        Public Company_Bank_Account As CompanyBankAccount

        Public Property Id As Integer

        Public Property Supplier_Id As Integer
        Public Property Supplier_Name As String
        Public Property Account_Number As String

        Public Property Company_Id As Integer
        Public Property Company_Name As String
        Public Property Company_Bank_Account_Id As Integer
        Public Property Company_Bank_Account_Number As String

        Public Property Default_Particulars As New ObservableCollection(Of ParticularsItem)

        Public Property ChangeState As States.ChangeState
        Sub New()

        End Sub
        Sub New(reader As MySql.Data.MySqlClient.MySqlDataReader)
            Id = reader.Item("id")
            Supplier_Id = reader.Item("supplier_id")
            Supplier_Name = reader.Item("supplier_name")

            Company_Id = reader.Item("company_id")
            Company_Name = reader.Item("Company_Name")

            Company_Bank_Account_Id = reader.Item("bank_account_id")
            Company_Bank_Account_Number = reader.Item("Company_Bank_Account_Number")

            Account_Number = reader.Item("account_number")
            Default_Particulars = JsonConvert.DeserializeObject(reader.Item("default_particulars"), Default_Particulars.GetType)
            ChangeState = States.ChangeState.None
        End Sub

        Public Overrides Function ToString() As String
            Return Account_Number
        End Function
    End Class

End Namespace
