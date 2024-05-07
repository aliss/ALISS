/*global document,location,window */
'use strict';

import attributelist from './attributelist';
import Parser from 'url-parse';
let currentIndex = 0,
    currentTitle = '',
    guideTitle = document.querySelector('.js-guide-title') ? document.querySelector('.js-guide-title').innerText : '',
    activeClassName = 'active',
    hash = location.hash.slice(1) || null,
    links = [].slice.call(document.querySelectorAll('.js-guide__link')),
    sections = [].slice.call(document.querySelectorAll('.js-guide__section')),
    incrementalNavHolder = document.querySelector('.js-guide__incremental'),
    incrementalNavData = {
        previous: {
            link: null,
            num: null,
            title: null
        },
        next: {
            link: null,
            num: null,
            title: null
        }
    };

const guide = () => {
    if(links && links.length > 0) {
        initialise();
        initialiseEditorLinks();
        setVisibility();
        setAria();
        renderIncrementalNav();
    } else {
        setVisibility();
    }
};

const initialise = () => {
    if(hash) window.location.hash = '';

    //get initial state from hash, or set to default
    links.forEach((link, i) => {
        if(link.getAttribute('href').substr(1) === hash) {
            currentIndex = i;
            currentTitle = link.innerText;
        }
    });
    return !currentTitle ? links[currentIndex].innerText : currentTitle;
};

const initialiseEditorLinks = () => {
    let articleLinks = [].slice.call(document.querySelectorAll('.js-guide__section a'));

    articleLinks.forEach(articleLink => {
        links.forEach(link => {
            var articleLinkHref = articleLink.getAttribute('href');
            if(articleLinkHref && link.getAttribute('href').substr(1) === articleLinkHref.split('#')[1]){
                articleLink.addEventListener('click', change);
                //console.log(articleLink.getAttribute('href'));
            }
        });
    });

};

//set current link from hash or default to first
const setVisibility = previousIndex => {
    if(previousIndex !== undefined) {
        links[previousIndex].classList.remove(activeClassName);
        sections[previousIndex].classList.remove(activeClassName);
    }
    if(links[currentIndex]) links[currentIndex].classList.add(activeClassName);
    sections[currentIndex].classList.add(activeClassName);
    window.scrollTo(0,0);
    window.setTimeout(() => {
        if(hash &&  window.location.hash === '')  {
            (!!window.history && !!window.history.pushState) && window.history.pushState({ URL: `#${hash}`}, '', `#${hash}`);
        }
        /*window.addEventListener('load', () => {
            window.location.hash = hash;
        });*/
        window.scrollTo(0,0);
    }, 0);

    pushToThirdParties(links[currentIndex]);
}

const pushToThirdParties = section => {
    //GA
    window.dataLayer = window.dataLayer || [];
    dataLayer.push({
        'guideName' : guideTitle,
        'guideItemName' : section ? section.innerText : null,
        'event' : 'guideItemEvent'
    });

    //social
    if(!document.querySelector('.js-grid-social') || !section) return;

    [].slice.call(document.querySelectorAll('.js-grid-social a')).forEach(link => {
        let parsedUrl = Parser(link.getAttribute('href'), true),
            paramName = parsedUrl.query['u'] ? 'u' : 'url',
            shareUrl = parsedUrl.query[paramName].split(''),
            replacedUrlPart = {};

        if(!!~shareUrl.indexOf('#')){
            shareUrl.splice(shareUrl.indexOf('#'));
        }
        replacedUrlPart[paramName] = `${shareUrl.join('')}#${section.innerText.split(' ').join('-').toLowerCase()}`
        parsedUrl.set('query', Object.assign(parsedUrl.query, replacedUrlPart));

        link.setAttribute('href', parsedUrl.href);
    });

};

//add ARIA to links - aria-selected, aria-controls
const setAria = () => {
    links.forEach((link, i) => {
        attributelist.set(link, {
            'aria-selected': currentIndex === i,
            'aria-controls': link.getAttribute('href').substr(1)
        });
    });
    
    //add ARIA to sections - aria-hidden
    sections.forEach((section, i) => {
        attributelist.set(section, {
            'aria-hidden': !(currentIndex === i)
        });
    });
};

const renderIncrementalNav = () => {
    let incrementalNav = '',
        getNavData = (i) => {
            return {
                link : links[i].href,
                num: i + 1,
                title: links[i].innerText
            }
        };
    
    //previous button
    if(currentIndex > 0) {
        incrementalNavData.previous = getNavData(currentIndex - 1);
        incrementalNav = `<a href="${incrementalNavData.previous.link}" rel="previous" class="js-guide__incremental--previous nav-incremental-link page-navigation__prev">
                <div class="nav-incremental__part">Part ${incrementalNavData.previous.num}</div>
                <div class="nav-incremental__title">${incrementalNavData.previous.title}</div>
            </div>`;
    }
    //next button
    if(currentIndex !== links.length - 1) {
        incrementalNavData.next = getNavData(currentIndex + 1);
        incrementalNav += `<a href="${incrementalNavData.next.link}" rel="next" class="js-guide__incremental--previous nav-incremental-link page-navigation__next">
                                <div class="nav-incremental__part">Part ${incrementalNavData.next.num}</div>
                                <div class="nav-incremental__title">${incrementalNavData.next.title}</div>
                            </a>`;
    }
    incrementalNavHolder.innerHTML = incrementalNav;
    
    bindEvents();
};

let bindEvents = () => {
    [].slice.call(document.querySelectorAll('.js-guide__incremental--previous, .js-guide__incremental--next, .js-guide__link')).forEach((btn) => {
        btn.addEventListener('click', change);
    });
};

let change = e => {
    e.preventDefault();
    let previousIndex = currentIndex;
    currentIndex = getNextIndex(`#${(e.target.parentNode.getAttribute('href') || e.target.getAttribute('href')).split('#')[1]}`);
    
    if(previousIndex === currentIndex) { return; }
    
    setVisibility(previousIndex);
    setAria();
    renderIncrementalNav();
    (!!window.history && !!window.history.pushState) && window.history.pushState({ URL: `#${(e.target.parentNode.getAttribute('href') || e.target.getAttribute('href')).split('#')[1]}`}, '', `#${(e.target.parentNode.getAttribute('href') || e.target.getAttribute('href')).split('#')[1]}`);
};

let getNextIndex = href => {
    let index = null;
    links.forEach((link, i) => {
        if(link.getAttribute('href') === href) {
            index = i;
        }
    });
    return index;
};

export default guide;