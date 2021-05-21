Namespace LanguageExtensions
    Module DataGridViewExtensions
        <DebuggerHidden()>
        <Runtime.CompilerServices.Extension()>
        Public Sub ExpandColumns(ByVal sender As DataGridView)
            For Each col As DataGridViewColumn In sender.Columns
                col.HeaderText = String.Join(" ", Text.RegularExpressions.Regex.Split(col.HeaderText, "(?=[A-Z])"))
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            Next
        End Sub
    End Module
End Namespace