Imports MySql.Data.MySqlClient

Namespace Model
    Public Class VoucherReminder
        Public Property Id As Integer
        Public Property Schedule_Id As Integer
        Public Schedule As VoucherScheduledEntry
        Public Property Schedule_Template_Name As String
        Public Property Schedule_Interval As VoucherScheduledEntry.IntervalChoices

        Public Property Template_Remarks As String

        Public Property Visible_Until As Date
        Public ReadOnly Property Day_Left As String
            Get
                Return (Now - Visible_Until).Days.ToString("D2")
            End Get
        End Property

        Public Property Date_Added As Date

        Public Property Status As Integer

        Public Property ChangeState As States.ChangeState

        Sub New()

        End Sub

        Sub New(interval As Integer, templatename As String, valuntil As Date, remarks As String)
            Schedule_Interval = interval
            Schedule_Template_Name = templatename
            Visible_Until = valuntil
            Template_Remarks = remarks
        End Sub

        Sub New(reader As MySql.Data.MySqlClient.MySqlDataReader)
            Id = reader.Item("id")
            Schedule_Id = reader.Item("Schedule_Id")
            Schedule_Template_Name = reader.Item("voucher_no")
            Schedule_Interval = reader.Item("Interval")
            Visible_Until = reader.Item("Visible_Until")
            Date_Added = reader.Item("Date_Added")
            'Status = reader.Item("status")
            Template_Remarks = reader.Item("remarks")

            ChangeState = States.ChangeState.None

        End Sub

        Public Overrides Function ToString() As String
            Return Id
        End Function
    End Class
End Namespace
