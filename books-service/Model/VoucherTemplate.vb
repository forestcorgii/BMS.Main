Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Model
    ''' <summary>
    ''' 
    ''' </summary>
    Public Class VoucherTemplate

        Public Property Id As Integer
        Public Property Supplier_Id As Integer
        Public Property Supplier_Account_Id As Integer
        Public Property Voucher As New Voucher

        Public Property ChangeState As States.ChangeState

        Sub New()
        End Sub

        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Voucher = JsonConvert.DeserializeObject(reader.Item("voucher_json"), Voucher.GetType)
            Supplier_Id = reader.Item("supplier_id")
            Supplier_Account_Id = reader.Item("supplier_account_id")

            ChangeState = States.ChangeState.None
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} - {1}", Supplier_Account_Id, Supplier_Id)
        End Function
    End Class
End Namespace