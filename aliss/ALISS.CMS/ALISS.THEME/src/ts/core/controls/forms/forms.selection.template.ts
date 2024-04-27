let Template = `
<button id="selected-##ID##" class="aliss-selected__remove" data-name="##NAME##" data-value="##VALUE##" data-input="category-##ID##" data-id="##ID##">
    <i class="fa fa-times-circle" aria-hidden="true"></i>
    <span class="hide">Remove ##VALUE##</span>
</button>
<span class="aliss-selected__value">##VALUE##</span>
`;

export default Template