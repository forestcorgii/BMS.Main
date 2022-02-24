Imports Newtonsoft.Json

Namespace Model
    Public Class JournalAccountDistributionItem
        Public Property Journal_Account As JournalAccount
        Public Property Amount As Double
        Public Property W_TAX As Double
        <JsonIgnore> Public ReadOnly Property W_TAX_Percentage
            Get
                If W_TAX > 0 Then Return W_TAX * 100
                Return 0
            End Get
        End Property

        Public Property VAT As String

        <JsonIgnore> Public ReadOnly Property Net_Amount As Double
            Get
                Return Amount - TAX_Amount
            End Get
        End Property

        <JsonIgnore> Public ReadOnly Property TAX_Amount As Double
            Get
                If VAT.ToUpper = "NONE" Then
                    Return Amount * W_TAX
                Else
                    Dim net_amount As Double
                    Select Case VAT.ToUpper
                        Case "NON-VAT", "VAT-EX" 'VAT Exclusive
                            net_amount = Amount * 0.12
                        Case "VAT-INC" 'VAT Inclusive
                            net_amount = (Amount / 1.12)
                    End Select

                    Return net_amount * W_TAX
                End If

                Return Nothing
            End Get
        End Property

    End Class

End Namespace
