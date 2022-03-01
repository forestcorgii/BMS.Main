Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Model
    Public Class VoucherScheduledEntry
        Public Property Id As Integer

        Public Property Template_Id As Integer
        Public Property Template_Name As String

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
            Template_Id = reader.Item("template_id")
            Template_Name = reader.Item("template_name")
            Interval = reader.Item("interval")

            Start_From = reader.Item("start_from")
            End_To = reader.Item("end_to")

            Last_Reminder_Generated = reader.Item("last_reminder_generated")

            ChangeState = States.ChangeState.None

        End Sub



        Public Overrides Function ToString() As String
            Return Id
        End Function

        Public Enum IntervalChoices
            Monthly
            Bi_Annual
            Annual
        End Enum
    End Class
End Namespace
