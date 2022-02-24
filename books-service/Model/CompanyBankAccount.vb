Imports MySql.Data.MySqlClient

Namespace Model

    Public Class CompanyBankAccount

        Public Property Id As Integer

#Region "Company Properties"
        Public Company As Company
        Public Property Company_Id As Integer
        Public Property Company_Name As String
#End Region

        Public Property Code As String
        Public Property Account_Number As String
        Public Property Name As String

        Public Property ChangeState As States.ChangeState
        Sub New()

        End Sub
        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Company_Id = reader.Item("company_id")
            Company_Name = reader.Item("company_name")
            Name = reader.Item("name")
            Code = reader.Item("code")
            Account_Number = reader.Item("account_number")

            ChangeState = States.ChangeState.None
        End Sub


        Public Overrides Function ToString() As String
            Return String.Format("{0} - {1}", Code, Account_Number)
        End Function
    End Class
End Namespace


