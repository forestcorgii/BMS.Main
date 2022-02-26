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

        Public Property VAT As VATTypeChoices

        <JsonIgnore> Public ReadOnly Property Net_Amount As Double
            Get
                Return Amount - TAX_Amount
            End Get
        End Property

        <JsonIgnore> Public ReadOnly Property TAX_Amount As Double
            Get
                If VAT = VATTypeChoices.NON_VAT Then
                    Return Amount * W_TAX
                Else
                    Dim net_amount As Double
                    Select Case VAT
                        Case VATTypeChoices.NON_VAT, VATTypeChoices.VAT_EX 'VAT Exclusive
                            net_amount = Amount * 0.12
                        Case VATTypeChoices.VAT_INC 'VAT Inclusive
                            net_amount = Amount / 1.12
                    End Select

                    Return net_amount * W_TAX
                End If

                Return Nothing
            End Get
        End Property


        Public Enum VATTypeChoices
            NON_VAT
            VAT_EX
            VAT_INC
        End Enum
    End Class
End Namespace
