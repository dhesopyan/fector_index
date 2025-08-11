@Code
    ViewData("Title") = "ExchangeTransaction"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action = "Index", "Exchange Transaction List", IIf(Action = "Approval", "Exchange Transaction Approval List", IIf(Action = "Review", "Exchange Transaction Review", IIf(Action = "Reactive", "Exchange Transaction Reactived", "Exchange Transaction History List"))))</h4>
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
                                @If Action = "Index" Or Action = "Approval" Or Action = "Review" Or Action = "Reactive" Then
                                    @<th class="hidden" id="filterReconsile">
                                        Reconsile
                                    </th>
                                Else
                                    @<th class="filter" id="filterReconsile">
                                        Reconsile
                                    </th>
                                End If
                                
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
                                @If Action = "Index" Or Action = "Approval" Or Action = "Review" Or Action = "Reactive" Then
                                    @<th class="hidden">
                                        Reconsile
                                    </th>
                                Else
                                    @<th class="hidden-xs filter">
                                        Reconsile
                                    </th>
                                End If
                                @If Action = "Review" Then
                                    @<th class="hidden-xs filter">
                                         Review
                                    </th>
                                Else
                                    @<th class="hidden-xs filter">
                                         Status
                                    </th>
                                End If
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
<style>
    .dataTables_filter, .dataTables_info {
        display: none;
    }
</style>
@Section scripts
    @If Action = "Index" Then
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExchangeTransactionCounterAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/exchangetransactioncounter-index.js")
    ElseIf Action = "Approval" Then
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExchangeTransactionCounterApprovalAjaxHandler")';

        </script>
        @Scripts.Render("~/Scripts/datatables/exchangetransactioncounter-approval.js")
    ElseIf Action = "History" Then
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExchangeTransactionHistoryAjaxHandler")';

        </script>
        @Scripts.Render("~/Scripts/datatables/exchangetransactionhistory.js")
    ElseIf Action = "Review" Then
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExchangeTransactionReviewAjaxHandler")';

        </script>
        @Scripts.Render("~/Scripts/datatables/exchangetransactionreview.js")
    ElseIf Action = "Reactive" Then
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExchangeTransactionReactiveAjaxHandler")';

        </script>
        @Scripts.Render("~/Scripts/datatables/exchangetransactionreactive.js")
    End If
<script>
    $(document).ready(function () {
        $('#datefilter').datepicker({
            dateFormat: "dd-mm-yy",
        });
    });
</script>
End Section
