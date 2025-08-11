@ModelType Fector_Index.MsPurpose
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "Purpose", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
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
                    <label>@Html.DisplayNameFor(Function(f) f.PurposeId)</label>
                    @Html.CustomTextBoxFor(Function(f) f.PurposeId, New With {.class = "form-control", .placeholder = "Purpose Code", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.PurposeId, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-12">
                    <label>@Html.DisplayNameFor(Function(f) f.Description)</label>
                    @Html.CustomTextBoxFor(Function(f) f.Description, New With {.class = "form-control", .placeholder = "Description"})
                    @Html.ValidationMessageFor(Function(f) f.Description, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "Purpose")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
    @Section scripts
    <script>
        $(document).ready(function(){
            if('@Action' == 'Edit')
                {
                    $('#PurposeId').attr('readonly','readonly');
                }
        });
    </script>
    End Section
</div>
