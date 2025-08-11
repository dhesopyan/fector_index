@ModelType Fector_Index.RptTransactionLimitView 
@Code
    ViewData("Title") = "Report Inquiry Limit"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim BranchList As IEnumerable(Of SelectListItem) = ViewBag.Branch

End Code

<div class="col-md-12">
    @Using Html.BeginForm("RptTransactionLimit", "Report", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "frmData"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Report Inquiry Limit</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-sm-12">
                    <label>@Html.DisplayNameFor(Function(f) f.Branch)</label>
                    <div class="form-group">
                        @Html.DropDownListFor(Function(f) f.Branch, BranchList, New With {.class = "form-control", .placeholder = "Branch"})
                        @Html.ValidationMessageFor(Function(f) f.Branch, String.Empty, New With {.class = "help-block"})
                    </div>
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnPrint" class="btn btn-primary"><i class="fa fa-print"></i> Print</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("IndexTransactionLimit", "Report")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
   </div>
