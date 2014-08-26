Option Explicit

Type COPYDATASTRUCT
    dwData As Long
    cbData As Integer
    lpData As String
End Type

Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As Long, ByVal lpWindowName As String) As Long

Public Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Long, ByVal wMsg As Long, ByVal wParam As Long, ByRef lParam As COPYDATASTRUCT) As Long

Const WM_COPYDATA = &H4A
Const LNULL = 0&

Public Function GetMUHwnd()
    Dim hwnd As Long
    Dim iCounter As Integer

    While hwnd = 0 And iCounter < 5
        hwnd = FindWindow(0, "Electronic Health Record Narrative")
        iCounter = iCounter + 1
    Wend

    GetMUHwnd = hwnd
End Function

Public Function SendEHRMessage(sMsg As String)
    Dim lReturn As Long
    Dim hwnd As Long
    Dim cds As COPYDATASTRUCT
    Dim iErrReturn As Integer

    hwnd = GetMUHwnd()

    If hwnd = 0 Then
        iErrReturn = MsgBox("Could Not find the EHRNarrative Window!", vbOkOnly, "Error!")
    Else
        cds.dwData = 0
        cds.cbData = Len(sMsg) + 1
        cds.lpData = sMsg
        lReturn = SendMessage(hwnd, WM_COPYDATA, LNULL, cds)
    End If
End Function

Sub Main()
    SendEHRMessage("This is from Dragon VBA")
End Sub