Imports System.ComponentModel
Imports MySql.Data.MySqlClient

Namespace Model

    ''' <summary>
    ''' Voucher
    ''' </summary>
    Public Class Company
        Implements ComponentModel.INotifyPropertyChanged

        Public Property Id As Integer
        Public Property Name As String
        Public Property Address As String
        Public Property TIN As String
        Public Property ChangeState As States.ChangeState
        Sub New()

        End Sub

        Sub New(reader As MySqlDataReader)
            Id = reader.Item("id")
            Name = reader.Item("name")
            Address = reader.Item("address")
            TIN = reader.Item("tin")

            ChangeState = States.ChangeState.None
        End Sub


        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Overrides Function ToString() As String
            Return String.Format("{0} - {1}", TIN, Name)
        End Function
    End Class
End Namespace
