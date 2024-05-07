let Template = `
<button id="selected-##ID##" class="aliss-selected__remove" data-name="##NAME##" data-value="##VALUE##" data-input="##IDTEXT##-##ID##" data-id="##ID##">
    <i class="fa fa-times-circle" aria-hidden="true"></i>
    <span class="hide">Remove ##VALUE##</span>
</button>
<span class="aliss-selected__value">
    <a target='_blank' href='https://www.google.com/maps/place/##VALUE##'><span style=padding-right:5px;">##VALUE##</span><i class="fas fa-external-link-alt" aria-hidden="true"></i></a>
</span>
`;

export default Template