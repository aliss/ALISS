let Template = `
<button id="selected-##ID##" class="aliss-selected__remove aliss-selected__remove--location" data-name="##NAME##" data-value="##VALUE##" data-input="location_##ID##" data-id="##ID##">
    <i class="fa fa-times-circle" aria-hidden="true"></i>
    <span class="hide">Remove ##VALUE##</span>
</button>
<span class="aliss-selected__value">
    <a target='_blank' href='https://maps.google.com?daddr=##VALUE##'><span style=padding-right:5px;">##VALUE##</span><i class="fa fa-external-link" aria-hidden="true"></i></a>
</span>
`;

export default Template