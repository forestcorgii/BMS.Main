Imports System.Collections.ObjectModel
Imports books_service

Public Class Reminderss
    Inherits ObservableCollection(Of Model.VoucherReminder)

    Sub New()
        Add(New Model.VoucherReminder(0, "Template 1", Now, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent id risus at ipsum interdum efficitur. Phasellus finibus erat nec lorem."))
        Add(New Model.VoucherReminder(1, "Template 2", Now, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent id risus at ipsum interdum efficitur. Phasellus finibus erat nec lorem."))
        Add(New Model.VoucherReminder(2, "Template 3", Now, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent id risus at ipsum interdum efficitur. Phasellus finibus erat nec lorem."))
        Add(New Model.VoucherReminder(3, "Template 4", Now, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent id risus at ipsum interdum efficitur. Phasellus finibus erat nec lorem."))
    End Sub
End Class
