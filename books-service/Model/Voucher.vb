Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports utility_service.Model

Namespace Model

    Public Class Voucher
        <JsonIgnore>
        Public Property Id As Integer
        <JsonIgnore> Public Property Voucher_No As String
        <JsonIgnore> Public Property Entry_Date As Date


        Public Particulars As New List(Of ParticularsItem)
        <JsonIgnore> Public Property Particulars_String As String
        Public Property Remarks As String

        <JsonIgnore> Public Prepared_By As User
        <JsonIgnore> Public Prepared_By_Fullname As String

        <JsonIgnore> Public Certified_By As User
        <JsonIgnore> Public Certified_By_Fullname As String

        <JsonIgnore> Public Approved_By As User
        <JsonIgnore> Public Approved_By_Fullname As String

        <JsonIgnore> Public Received_By As String

        <JsonIgnore> Public Property Print_Status As PrintStatusChoices

#Region "Company"
        <JsonIgnore> Public Company As Company
        <JsonIgnore> Public Property Company_Name As String
        Public Property Company_Id As Integer
#End Region

#Region "Company's Bank Account"
        <JsonIgnore> Public Bank_Account As CompanyBankAccount
        <JsonIgnore> Public Property Bank_Account_Code As String
        Public Property Bank_Account_Id As Integer
#End Region

#Region "Supplier"
        <JsonIgnore> Public Supplier As Supplier
        Public Property Supplier_Id As Integer
        <JsonIgnore> Public Property Supplier_Payee As String

#End Region

#Region "Supplier's Account"
        <JsonIgnore> Public Supplier_Account As SupplierAccount
        Public Property Supplier_Account_Id As Integer
        <JsonIgnore> Public Property Supplier_Account_Number As String
#End Region

#Region "Journal Account"
        Public Journal_Account_Distributions As New List(Of JournalAccountDistributionItem)
        <JsonIgnore> Public Property Journal_Account_Distributions_String As String
#End Region
        <JsonIgnore> Public Property ChangeState As States.ChangeState


#Region "Computations"
        <JsonIgnore> Public ReadOnly Property Gross_Amount As Double
            Get
                Dim _amount As Double = 0
                If Journal_Account_Distributions.Count > 0 Then
                    For Each j As JournalAccountDistributionItem In Journal_Account_Distributions
                        _amount += j.Amount
                    Next
                End If
                Return _amount
            End Get
        End Property
        <JsonIgnore> Public ReadOnly Property Net_Amount As Double
            Get
                Return Gross_Amount - Tax_Amount
            End Get
        End Property
        <JsonIgnore> Public ReadOnly Property Net_Amount_Words As String
            Get
                Return Utils.AmountInWords(Net_Amount)
            End Get
        End Property
        <JsonIgnore> Public ReadOnly Property Tax_Amount As Double
            Get
                Dim total_tax_amount As Double
                If Journal_Account_Distributions.Count > 0 Then
                    For Each j As JournalAccountDistributionItem In Journal_Account_Distributions
                        total_tax_amount += j.TAX_Amount
                    Next
                End If
                Return total_tax_amount
            End Get
        End Property
#End Region


        Sub New()
        End Sub

        Sub New(reader As MySql.Data.MySqlClient.MySqlDataReader)
            Id = reader.Item("id")
            Voucher_No = reader.Item("voucher_no")
            Entry_Date = reader.Item("Entry_Date")
            Company_Name = reader.Item("Company_Name")
            Supplier_Payee = reader.Item("Supplier_Payee")
            Supplier_Account_Number = reader.Item("Supplier_Account_Number")
            Particulars_String = reader.Item("particulars")
            Particulars = JsonConvert.DeserializeObject(Particulars_String, Particulars.GetType)
            Journal_Account_Distributions_String = reader.Item("journal_account_distributions")
            Journal_Account_Distributions = JsonConvert.DeserializeObject(Journal_Account_Distributions_String, Journal_Account_Distributions.GetType)
            Bank_Account_Code = reader.Item("bank_Account_Code")
            Remarks = reader.Item("Remarks")
            Print_Status = reader.Item("print_status")

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
