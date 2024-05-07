
let Template = {
	"title": `        
    <h2 class="d-inline-block">
        <a id="aliss-search-reults-service-link-##ID##" href="/services/##SLUG##" class="aliss-component-master__contents__primary-link hide-print no-margin">
            ##NAME##
        </a>
    </h2>`,
	"serviceClaimed": `
        <span title="This is a claimed service" class="aliss-claimed-status aliss-claimed-status--claimed"><span class="sr-only">This is a </span>Claimed<span class="sr-only"> service</span></span>
    `,
	"serviceNotClaimed": `
        <span title="This is an unclaimed service" class="aliss-claimed-status aliss-claimed-status--unclaimed align-bottom"><span class="sr-only">This is an </span>Unclaimed<span class="sr-only"> service</span></span>
    `,
	"DeliveredBy": `
        <strong>Delivered by:</strong>
        <a class="aliss-component-master__contents__primary-link no-margin" href="/organisations/##ORGSLUG##">##ORGNAME##</a>
    `,
	"orgClaimed": `
        <span title="This is a claimed service" class="aliss-claimed-status aliss-claimed-status--claimed"><span class="sr-only">This is a </span>Claimed<span class="sr-only"> service</span></span>
    `,
	"orgNotClaimed": `
        <span title="This is an unclaimed organisation." class="aliss-claimed-status aliss-claimed-status--unclaimed"><span class="sr-only">This is an </span>Unclaimed<span class="sr-only"> organisation</span></span>
    `,
    "reviewDate": `
		<span><i class="fa fa-calendar-check-o" aria-hidden="true"></i><strong> Last reviewed: </strong>##LASTREVIEWED##</span>
    `,
	"summary": `
        <p>##SUMMARY##</p>
    `,
	"webLink":`            
        <li class="aliss-icon-list__item aliss-icon-list__item--website hide-print">
            <a href="##WEB##" title="This will open in a new window" target="_blank">
                Visit the website
            </a>
        </li>
        <li class="aliss-icon-list__item aliss-icon-list__item--website show-print">
            <span>##WEB##</span>
            <br>
        </li>
    `,
	"referral":`
        <li class="aliss-icon-list__item aliss-icon-list__item--referral hide-print">
            <a href="##REF##" title="This will open in a new window" target="_blank">
                Referral Information
            </a>
        </li>
        <li class="aliss-icon-list__item aliss-icon-list__item--referral show-print">
            <span>##REF##</span>
            <br />
        </li>
    `,
	"phone":`            
        <li class="aliss-icon-list__item aliss-icon-list__item--telephone">
            <a href="##PHONE##" title="Click here to call this service">
                ##PHONE##
            </a>
        </li>
    `,
	"email":`
        <li class="aliss-icon-list__item aliss-icon-list__item--email">
            <a href="mailto:##EMAIL##" title="This will open your default email client" target="_blank">
                <span class="sr-only">contact ##EMAIL## by email at - </span>##EMAIL##
            </a>
        </li>
    `,
};

export default Template 