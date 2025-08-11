@Code
    ViewData("Title") = "Document Underlying"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action = "Index", "Document Underlying List", "Document Underlying Approval List")</h4>
            </div>
            <div class="pull-right">
                @If Action = "Index" Then
                    @<a href="~/DocumentTransaction/Create" class="btn btn-primary"><i class="fa fa-plus"></i>&nbsp;Create</a>
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
                                <th class="hidden">
                                    &nbsp;
                                </th>
                                <th class="hidden-xs filter">
                                    Description
                                </th>
                                <th class="hidden-xs filter">
                                    Document Type
                                </th>
                                <th class="hidden-xs filter">
                                    Customer Type
                                </th>
                                <th class="hidden-xs filter" id="filterDDL">
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
                                <th class="hidden">
                                    Document ID
                                </th>
                                <th class="hidden-xs filter">
                                    Description
                                </th>
                                <th class="hidden-xs filter">
                                    Document Type
                                </th>
                                <th class="hidden-xs filter">
                                    Customer Type
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
            var ajaxHandler = '@Url.Action("DTDocumentTransactionAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/documenttransaction-index.js")
    Else
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTDocumentTransactionApprovalAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/documenttransaction-approval.js")
    End If

End Section
