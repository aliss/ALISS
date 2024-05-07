let Template = `
<button id="selected-##ID##-##LOCATION##" class="aliss-selected__remove aliss-selected__remove-##LOCATION##" data-name="##NAME##" data-value="##VALUE##" data-input="##IDTEXT##-##ID##-##LOCATION##" data-id="##ID##">
    <i class="fa fa-times-circle" aria-hidden="true"></i>
    <span class="hide">Remove ##VALUE##</span>
</button>
<span class="aliss-selected__value">##VALUE##</span>
`;

export default Template