Imports MySql.Data.MySqlClient

Namespace Model


    Public Class Supplier
        Public Property Id As Integer

        Public Property Name As String
        Public Property Payee As String

        Public Property Address As String
        Public Property TIN As String
        Public Property Remarks As String

        Public Property ChangeState As States.ChangeState

        Sub New()

        End Sub

        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Name = reader.Item("name")
            Payee = reader.Item("payee")
            Address = reader.Item("address")
            TIN = reader.Item("tin")
            Remarks = reader.Item("remarks")

            ChangeState = States.ChangeState.None
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} - {1}", Name, TIN)
        End Function
    End Class
End Namespace
