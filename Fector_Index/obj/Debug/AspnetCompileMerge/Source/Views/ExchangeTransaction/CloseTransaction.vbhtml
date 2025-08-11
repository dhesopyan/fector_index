@ModelType Fector_Index.ExchangeTransactionClose 
@Code
    ViewData("Title") = "CloseTransaction"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>Close Transaction</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter hidden">
                                    Transaction Number
                                </th>
                                <th class="hidden-xs filter">
                                    Deal Number
                                </th>
                                <th class="filter" data-id="datefilter">
                                    Date
                                </th>
                                <th class="hidden-xs filter">
                                    Account Number
                                </th>
                                <th class="hidden-xs filter">
                                    Account Name
                                </th>
                                <th class="hidden-xs filter">
                                    Transaction Type
                                </th>
                                <th class="hidden-xs filter">
                                    Source Funds
                                </th>
                                <th class="hidden" id="filterReconsile">
                                        Reconsile
                                    </th>
                                <th class="filter" id="filterDDL">
                                    Status
                                </th>
                                <th>
                                    <a id="BtnFilter" class="btn btn-primary btn-sm">
                                        <span class="fa fa-search"></span>
                                    </a>
                                    <a id="BtnClear" class="btn btn-primary btn-sm">
                                        <span class="fa fa-refresh"></span>
                                    </a>
                                </th>
                                <th class="hidden">
                                    BranchId
                                </th>
                            </tr>
                            <tr id="GvHeader" class="GvHeader">
                                <th class="filter hidden">
                                    Transaction Number
                                </th>
                                <th class="hidden-xs filter">
                                    Deal Number
                                </th>
                                <th class="hidden-xs filter">
                                    Date
                                </th>
                                <th class="hidden-xs filter">
                                    Account Number
                                </th>
                                <th class="hidden-xs filter">
                                    Account Name
                                </th>
                                <th class="hidden-xs filter">
                                    Transaction Type
                                </th>
                                <th class="hidden-xs filter">
                                    Source Funds
                                </th>
                                <th class="hidden">
                                    Reconsile
                                </th>
                                <th class="hidden-xs filter">
                                    Status
                                </th>
                                <th>&nbsp;</th>
                                <th class="hidden">
                                    BranchId
                                </th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
@Using Html.BeginForm("CloseTransaction", "ExchangeTransaction", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData"})
    @<div id="Modal" class="modal fade" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Close Transaction</h4>
                </div>
                <div class="modal-body" style="height: 200px;">
                    <div class="col-md-12 hidden ">
                        <label>@Html.DisplayNameFor(Function(f) f.TransNum)</label>
                        @Html.CustomTextBoxFor(Function(f) f.TransNum, New With {.class = "form-control", .placeholder = "Transnum", .style = "text-transform:uppercase"})
                        @Html.ValidationMessageFor(Function(f) f.TransNum, String.Empty, New With {.class = "help-block"})
                    </div>
                    <div class="col-md-12 ">
                        <label>@Html.DisplayNameFor(Function(f) f.DealNumber)</label>
                        @Html.CustomTextBoxFor(Function(f) f.DealNumber, New With {.class = "form-control", .placeholder = "Deal Number", .style = "text-transform:uppercase"})
                        @Html.ValidationMessageFor(Function(f) f.DealNumber, String.Empty, New With {.class = "help-block"})
                    </div>
                    <div class="col-md-12 ">
                        <label>@Html.DisplayNameFor(Function(f) f.CloseTransactionReason)</label>
                        @Html.CustomTextBoxFor(Function(f) f.CloseTransactionReason, New With {.class = "form-control", .placeholder = "Close Transaction Reason", .style = "text-transform:uppercase"})
                        @Html.ValidationMessageFor(Function(f) f.CloseTransactionReason, String.Empty, New With {.class = "help-block"})
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" id="btnSave" value="Save"><i class="fa fa-check"></i> Save</button>
                    <button class="btn btn-danger" id="btnCancel" data-dismiss="modal" value="Cancel"><i class="fa fa-close"></i> Cancel</button>
                </div>
            </div>
        </div>
    </div>
End Using
<style>
    .dataTables_filter, .dataTables_info {
        display: none;
    }
</style>
@Section scripts
    <script>
        $(document).ready(function () {
            $('#datefilter').datepicker({
                dateFormat: "dd-mm-yy",
            });
        });

        var baseUrl = '@Url.Content("~")';
        var ajaxHandler = '@Url.Action("DTExchangeTransactionClosedAjaxHandler")';

    </script>
    @Scripts.Render("~/Scripts/datatables/exchangetransactionclosed.js")
End Section
