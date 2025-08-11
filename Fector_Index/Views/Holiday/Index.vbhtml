@Code
    ViewData("Title") = "Holiday"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>Holiday List</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter hidden ">
                                    Holiday Id
                                </th>
                                <th class="filter">
                                    Holiday Description
                                </th>
                                <th class="filter" data-id="datefilter">
                                    Start Date
                                </th>
                                <th class="filter" data-id="datefilter2">
                                    End Date
                                </th>
                                <th class="filter">
                                    Nostro
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
                                    Holiday Id
                                </th>
                                <th>
                                    Holiday Description
                                </th>
                                <th>
                                    Start Date
                                </th>
                                <th>
                                    End Date
                                </th>
                                <th>
                                    Nostro
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
        var ajaxHandler = '@Url.Action("DTHolidayAjaxHandler")';
    </script>
    @Scripts.Render("~/Scripts/datatables/holiday-index.js")

<script>
    $(document).ready(function () {
        $('#datefilter').datepicker({
            dateFormat: "dd-mm-yy",
        });
        $('#datefilter2').datepicker({
            dateFormat: "dd-mm-yy",
        });
    });
</script>

End Section
