@Code
    ViewData("Title") = "TransactionDeal"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action = "Index", "Transaction Deal List", "Transaction Deal Approval List")</h4>
            </div>
            <div class="pull-right">
                @*@If Action = "Index" Then
                    @<a href="~/TransactionDeal/Create" class="btn btn-primary"><i class="fa fa-plus"></i>&nbsp;Create</a>
                End If*@
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
                                    ID
                                </th>
                                <th class="filter">
                                    Deal Num.
                                </th>
                                <th class="filter">
                                    Acc. Num.
                                </th>
                                <th class="filter">
                                    Acc. Name
                                </th>
                                <th class="filter">
                                    Branch
                                </th>
                                <th class="filter">
                                    Trans. Type
                                </th>
                                <th class="filter">
                                    Curr. Deal
                                </th>
                                @*<th class="hidden-xs filter">
                                    Amount Deal
                                </th>
                                <th class="hidden-xs filter">
                                    Currency Customer
                                </th>*@
                                <th >
                                    &nbsp; 
                                </th>
                                <th class="filter" data-id="datefilter">
                                    Value Date
                                </th>
                                <th class="hidden-xs filter" id="filterDDL">
                                    Status
                                </th>
                                <th>
                                    <a id="BtnFilter" class="btn btn-default btn-sm">
                                        <span class="fa fa-search"></span>
                                    </a>
                                    <a id="BtnClear" class="btn btn-default btn-sm">
                                        <span class="fa fa-refresh"></span>
                                    </a>
                                </th>
                            </tr>
                            <tr id="GvHeader" class="GvHeader">
                                <th class="hidden filter">
                                    ID
                                </th>
                                <th class="hidden-xs filter">
                                    Deal Num.
                                </th>
                                <th class="hidden-xs filter">
                                    Acc. Num.
                                </th>
                                <th class ="hidden-xs filter">
                                    Acc. Name
                                </th>
                                <th class="hidden-xs filter">
                                    Branch
                                </th>
                                <th class="hidden-xs filter">
                                    Trans. Type
                                </th>
                                <th class="hidden-xs filter">
                                    Curr. Deal
                                </th>
                                @*<th class="hidden-xs filter">
                                    Amount Deal
                                </th>
                                <th class="hidden-xs filter">
                                    Currency Customer
                                </th>*@
                                <th class="hidden-xs filter">
                                    Amount Customer
                                </th>
                                <th class="hidden-xs filter">
                                    Value Date
                                </th>
                                <th class="hidden-xs filter">
                                    Status
                                </th>
                                <th>&nbsp;</th>
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
            var ajaxHandler = '@Url.Action("DTDealAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/transactiondeal-index.js")
    ElseIf Action = "Approval" Then
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTDealApprovalAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/transactiondeal-approval.js")
    Else
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTDealHistoryAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/transactiondeal-history.js")
    End If

<script>
    $(document).ready(function () {
        $('#datefilter').datepicker({
            dateFormat: "dd-mm-yy",
        });
    });
</script>
End Section
