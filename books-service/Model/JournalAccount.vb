Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Namespace Model

    Public Class JournalAccount
        <JsonIgnore> Public Property Id As Integer
        Public Property Name As String

        Private _Rate As Double
        <JsonIgnore> Private _Rate_Percentage As Integer

        <JsonIgnore> Public Property Rate As Double
            Get
                Return _Rate
            End Get
            Set(value As Double)
                _Rate = value
                _Rate_Percentage = value * 100
            End Set
        End Property
        <JsonIgnore> Public Property Rate_Percentage As Double
            Get
                Return _Rate_Percentage
            End Get
            Set(value As Double)
                _Rate_Percentage = value
                _Rate = value / 100
            End Set
        End Property

        <JsonIgnore> Public Property ChangeState As States.ChangeState

        Sub New()
        End Sub

        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Name = reader.Item("name")
            Rate = reader.Item("rate")

            ChangeState = States.ChangeState.None
        End Sub


        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class
End Namespace
