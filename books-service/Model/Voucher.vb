Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports utility_service.Model

Namespace Model

    Public Class Voucher

        Public Property Id As Integer
        Public Property Voucher_No As String
        Public Property Entry_Date As Date


        Public Particulars As New List(Of ParticularsItem)
        Public Property Particulars_String As String
        Public Property Remarks As String

        Public Prepared_By As User
        Public Prepared_By_Fullname As String

        Public Certified_By As User
        Public Certified_By_Fullname As String

        Public Approved_By As User
        Public Approved_By_Fullname As String

        Public Received_By As String

        Public Property Print_Status As PrintStatusChoices

#Region "Company"
        Public Company As Company
        Public Property Company_Name As String
        Public Property Company_Id As Integer
#End Region

#Region "Company's Bank Account"
        Public Bank_Account As CompanyBankAccount
        Public Property Bank_Account_Code As String
        Public Property Bank_Account_Id As Integer
#End Region

#Region "Supplier"
        Public Supplier As Supplier
        Public Property Supplier_Id As Integer
        Public Property Supplier_Payee As String

#End Region

#Region "Supplier's Account"
        Public Supplier_Account As SupplierAccount
        Public Property Supplier_Account_Id As Integer
        Public Property Supplier_Account_Number As String
#End Region

#Region "Journal Account"
        Public Journal_Account_Distributions As New List(Of JournalAccountDistributionItem)
        Public Property Journal_Account_Distributions_String As String
#End Region
        Public Property ChangeState As States.ChangeState


#Region "Computations"
        Public Function Gross_Amount() As Double
            Dim _amount As Double = 0
            If Journal_Account_Distributions.Count > 0 Then
                For Each j As JournalAccountDistributionItem In Journal_Account_Distributions
                    _amount += j.Amount
                Next
            End If
            Return _amount
        End Function
        Public Function Net_Amount() As Double
            Return Gross_Amount - Tax_Amount()
        End Function
        Public Function Net_Amount_Words() As String
            Return Utils.AmountInWords(Net_Amount)
        End Function
        Public Function Tax_Amount() As Double
            Dim total_tax_amount As Double
            If Journal_Account_Distributions.Count > 0 Then
                For Each j As JournalAccountDistributionItem In Journal_Account_Distributions
                    total_tax_amount += j.TAX_Amount
                Next
            End If
            Return total_tax_amount
        End Function
#End Region


        Sub New()
        End Sub

        Sub New(reader As MySql.Data.MySqlClient.MySqlDataReader, Optional asTemplate As Boolean = False)
            If asTemplate = False Then
                Id = reader.Item("id")
                Voucher_No = reader.Item("voucher_no")
                Entry_Date = reader.Item("Entry_Date")
                Print_Status = reader.Item("print_status")
            End If

            Particulars_String = reader.Item("particulars")
            Journal_Account_Distributions_String = reader.Item("journal_account_distributions")
            Particulars = JsonConvert.DeserializeObject(Particulars_String, Particulars.GetType)
            Journal_Account_Distributions = JsonConvert.DeserializeObject(Journal_Account_Distributions_String, Journal_Account_Distributions.GetType)

            Remarks = reader.Item("Remarks")

            Company_Id = reader.Item("company_id")
            Bank_Account_Id = reader.Item("bank_account_id")
            Supplier_Id = reader.Item("supplier_id")
            Supplier_Account_Id = reader.Item("supplier_account_id")
            ChangeState = States.ChangeState.None

        End Sub

        Public Enum PrintStatusChoices
            NOT_PRINTED
            QUEUED_FOR_PRINT
            PRINTED
            RE_PRINTED
            CANCELLED
        End Enum
    End Class



End Namespace
