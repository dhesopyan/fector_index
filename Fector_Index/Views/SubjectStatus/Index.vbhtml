@Code
    ViewData("Title") = "Subject Status"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>Subject Status List</h4>
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
                                    Status Code
                                </th>
                                <th class="filter">
                                    Description
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
                                    Status Code
                                </th>
                                <th >
                                    Description
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
        <script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTSubjectStatusAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/subjectstatus-index.js")
End Section
