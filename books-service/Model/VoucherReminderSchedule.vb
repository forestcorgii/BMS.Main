Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Model
    Public Class VoucherScheduledEntry
        Public Property Id As Integer

        Public Property Voucher As Voucher
        Public Property Voucher_Id As Integer
        Public Property Voucher_No As String

        Public Property Interval As IntervalChoices

        Public Property Start_From As Date
        Public Property End_To As Date

        Public Property Last_Entry As Date
        Public Property Next_entry As Date

        Public Property Last_Reminder_Generated As Date

        Public Property ChangeState As States.ChangeState


        Sub New()

        End Sub
        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Voucher_Id = reader.Item("voucher_id")
            Voucher_No = reader.Item("voucher_no")

            Interval = reader.Item("interval")

            Start_From = reader.Item("start_from")

            Last_Reminder_Generated = reader.Item("last_reminder_generated")

            ChangeState = States.ChangeState.None

        End Sub



        Public Overrides Function ToString() As String
            Return Voucher_No
        End Function

        Public Enum IntervalChoices
            Monthly
            Bi_Annual
            Annual
        End Enum
    End Class
End Namespace
