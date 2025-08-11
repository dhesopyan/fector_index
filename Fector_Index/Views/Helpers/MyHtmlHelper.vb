Imports System.ComponentModel.DataAnnotations
Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module MyHtmlHelper

        <Extension()> _
        Public Function CustomTextBoxFor(Of TModel, TProperty)(htmlHelper As HtmlHelper(Of TModel), expression As Expression(Of Func(Of TModel, TProperty)), htmlAttributes As Object) As MvcHtmlString
            Dim member = TryCast(expression.Body, MemberExpression)
            Dim stringLength = TryCast(member.Member.GetCustomAttributes(GetType(StringLengthAttribute), False).FirstOrDefault(), StringLengthAttribute)

            Dim attributes = DirectCast(New RouteValueDictionary(htmlAttributes), IDictionary(Of String, Object))
            If stringLength IsNot Nothing Then
                attributes.Add("maxlength", stringLength.MaximumLength)
            End If
            Return htmlHelper.TextBoxFor(expression, attributes)
        End Function

        <Extension()> _
        Public Function CustomPasswordFor(Of TModel, TProperty)(htmlHelper As HtmlHelper(Of TModel), expression As Expression(Of Func(Of TModel, TProperty)), htmlAttributes As Object) As MvcHtmlString
            Dim member = TryCast(expression.Body, MemberExpression)
            Dim stringLength = TryCast(member.Member.GetCustomAttributes(GetType(StringLengthAttribute), False).FirstOrDefault(), StringLengthAttribute)

            Dim attributes = DirectCast(New RouteValueDictionary(htmlAttributes), IDictionary(Of String, Object))
            If stringLength IsNot Nothing Then
                attributes.Add("maxlength", stringLength.MaximumLength)
            End If
            Return htmlHelper.PasswordFor(expression, attributes)
        End Function

    End Module

End Namespace
