@ModelType Fector_Index.MsBICode 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim Status As SelectList = ViewBag.Status 
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "BICode", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Purpose Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.BICode)</label>
                    @Html.CustomTextBoxFor(Function(f) f.BICode, New With {.class = "form-control", .placeholder = "BI Code", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.BICode, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.SubjectStatusId)</label>
                    @Html.DropDownListFor(Function(f) f.SubjectStatusId, Status, "-- SELECT STATUS --", New With {.class = "form-control", .placeholder = "Status"})
                    @Html.ValidationMessageFor(Function(f) f.SubjectStatusId, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.SubjectName )</label>
                    @Html.CustomTextBoxFor(Function(f) f.SubjectName, New With {.class = "form-control", .placeholder = "Subject Name", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.SubjectName, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "BICode")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
    @Section scripts
        <script>
        $(document).ready(function(){
            if('@Action' == 'Edit')
                {
                    $('#BICode').attr('readonly','readonly');
                }
        });
        </script>
    End Section
</div>
