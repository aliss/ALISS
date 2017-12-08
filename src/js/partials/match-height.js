import 'jquery-match-height';
import throttle from 'lodash.throttle';
let w = window.innerWidth;

let doesExist = (el) => {
    return el.length > 0 ? true : false;
}
let matchHeight = () => {

   if (doesExist($('.for-not-for .info-box'))) {
       $('.for-not-for .info-box').matchHeight({byRow: true});
   }
   if (doesExist($('.cells a'))) {
       $('.cells a').matchHeight({byRow: true});
   }

};
export default matchHeight;