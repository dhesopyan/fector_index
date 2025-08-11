@Code
    ViewData("Title") = "Purpose"
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code
<div class="row col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>@IIf(Action = "Index", "Purpose List", "Purpose Approval List")</h4>
            </div>
            <div class="pull-right">
                @If Action = "Index" Then
                    @<a href="~/Purpose/Create" class="btn btn-primary"><i class="fa fa-plus"></i>&nbsp;&nbsp;Create</a>    
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
                                    Purpose Code
                                </th>
                                <th class="filter">
                                    Description
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
                                <th>
                                    Purpose Code
                                </th>
                                <th>
                                    Description
                                </th>
                                <th>
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
            var ajaxHandler = '@Url.Action("DTPurposeAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/purpose-index.js")
    Else
        @<script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTPurposeApprovalAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/purpose-approval.js")
    End If
    
End Section
