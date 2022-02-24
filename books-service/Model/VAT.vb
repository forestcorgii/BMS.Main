Namespace Model
    Public Class VAT
        Public Property Name As String
        Public Property Rate As Double


        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace
