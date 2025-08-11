@ModelType Fector_Index.MsMappingNostro 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Dim strBIC As IEnumerable(Of SelectListItem) = ViewBag.strBIC
    Dim Currency As IEnumerable(Of SelectListItem) = ViewBag.Currency
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "MappingNostro", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Mapping Nostro Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.BIC)</label>
                    @Html.DropDownListFor(Function(f) f.BIC, strBIC, "-- SELECT NOSTRO --", New With {.class = "form-control", .placeholder = "NOSTRO"})
                    @Html.ValidationMessageFor(Function(f) f.BIC, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.CurrID)</label>
                    @Html.DropDownListFor(Function(f) f.CurrID, Currency, "-- SELECT CURRENCY --", New With {.class = "form-control", .placeholder = "Currency"})
                    @Html.ValidationMessageFor(Function(f) f.CurrID, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "MappingNostro")"><i class="fa fa-arrow-left"></i> Back</a>
            </div>
            <div class="clearfix"></div>
        </div>
    End Using
</div>
