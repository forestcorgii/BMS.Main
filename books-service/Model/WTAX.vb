Namespace Model
    Public Class WTAX
        'Public Property Id As Integer
        Public Property Name As String
        Public Property Rate As Double


        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

End Namespace
