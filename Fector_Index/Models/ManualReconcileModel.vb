Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class ManualReconcileViewModel
    <DisplayName("Deal Number")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="Deal Number cannot be empty")>
    Public Property DealNumber As String

    <DisplayName("Date")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="Date cannot be empty")>
    Public Property TDate As DateTime

    <DisplayName("Core Reference Number")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="Core Reference Number cannot be empty")>
    Public Property CoreReferenceNumber As String

    <DisplayName("Branch")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="Branch cannot be empty")>
    Public Property BranchId As String

    <DisplayName("Account Number")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="Account Number value cannot be empty")>
    Public Property AccountNumber As String

    <DisplayName("Account Name")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="Account Name cannot be empty")>
    Public Property AccountName As String

    <Required(AllowEmptyStrings:=False, ErrorMessage:="Currency value cannot be empty")>
    Public Property Currency As String

    <Required(AllowEmptyStrings:=False, ErrorMessage:="Enter vale of ammount")>
    Public Property Amount As String

    Public Property Time As String
End Class


