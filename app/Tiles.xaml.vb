Public Class Tiles

    Public Property Source As ImageSource
        Get
            Return imgIcon.Source
        End Get
        Set(value As ImageSource)
            imgIcon.Source = value
        End Set
    End Property
    Public Property Header As String
        Get
            Return lbHeader.Text
        End Get
        Set(value As String)
            lbHeader.Text = value
        End Set
    End Property

    Public Event Click(sender As Object, e As RoutedEventArgs)

    Private Sub btn_Click(sender As Object, e As RoutedEventArgs)
        RaiseEvent Click(sender, e)
    End Sub
End Class
