Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Model
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class VoucherTemplate

        Public Property Id As Integer
        Public Property Name As String
        Public Property Remarks As String
        Public Property Voucher As New Voucher

        Public Property ChangeState As States.ChangeState

        Sub New()
        End Sub

        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Voucher = JsonConvert.DeserializeObject(reader.Item("voucher_json"), Voucher.GetType)
            Name = reader.Item("name")
            Remarks = reader.Item("remarks")

            ChangeState = States.ChangeState.None
        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace