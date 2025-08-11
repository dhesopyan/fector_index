@Code
    ViewData("Title") = "Extract Setting Transaction"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action = "Index", "Extract Setting Transaction List", "Extract Setting Transaction Approval List")</h4>
            </div>
            <div class="pull-right">
                @If Action = "Index" Then
                    @<a href="~/ExtractSettingTransaction/Create" class="btn btn-primary"><i class="fa fa-plus"></i>&nbsp;Create</a>
                End If
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter">
                                    COA
                                </th>
                                <th class="filter">
                                    Transaction Code
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
                            </tr>
                            <tr id="GvHeader" class="GvHeader">
                                <th class="filter">
                                    COA
                                </th>
                                <th class="filter">
                                    Transaction Code
                                </th>
                                <th class="filter">
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
            var ajaxHandler = '@Url.Action("DTExtractSettingTransactionAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/ExtractSettingTransaction-index.js")
    Else
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTExtractSettingTransactionApprovalAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/ExtractSettingTransaction-approval.js")
    End If

End Section
