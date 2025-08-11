@Code
    ViewData("Title") = "MappingDocument"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action = "Index", "Mapping Document List", "Mapping Document Approval List")</h4>
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
                                    LHBU Purpose
                                </th>
                                <th class="filter">
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
                                <th class="filter">
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
                                    LHBU Purpose
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
            var ajaxHandler = '@Url.Action("DTMappingDocumentAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/mappingdocument-index.js")
    Else
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTMappingDocumentApprovalAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/mappingdocument-approval.js")
    End If

End Section
