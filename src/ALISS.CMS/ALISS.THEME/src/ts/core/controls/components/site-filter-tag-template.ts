
let FilterTagTemplate = {
	"default": `
	<label id="selected-##DATANAME##--##SLUG##" class="aliss-selected__remove aliss-selected-##DATANAME##">
		<i class="fa fa-times-circle" aria-hidden="true"></i>
		<span class="sr-only">Click here to remove ##SLUG##</span>
	</label>
	<span class="aliss-selected__value">##NAME##</span>
	`,
	"postcode": `
	<label id="selected-postcode" class="aliss-selected__remove">
		<i class="fa fa-times-circle" aria-hidden="true"></i>
		<span class="sr-only">Click here to remove the postcode</span>
	</label>
	<span id="selected-postcode-value" class="aliss-selected__value">##POSTCODE##</span>
	`,
	"what": `
	<label id="selected-what" class="aliss-selected__remove">
		<i class="fa fa-times-circle" aria-hidden="true"></i>
		<span class="sr-only">Click here to remove the what</span>
	</label>
	<span id="selected-what-value" class="aliss-selected__value">##WHAT##</span>
	`,
}

export default FilterTagTemplate 