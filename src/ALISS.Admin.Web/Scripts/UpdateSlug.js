function UpdateSlug() {
    var categoryName = document.getElementById('Name').value;
    categoryName = categoryName.replace(/^\s+|\s+$/g, '');
    categoryName = categoryName.toLowerCase();

    // remove accents, swap ñ for n, etc
    var from = "àáãäâèéëêìíïîòóöôùúüûñç·/_,:;";
    var to = "aaaaaeeeeiiiioooouuuunc------";
    for (var i = 0, l = from.length; i < l; i++) {
        categoryName = categoryName.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
    }

    categoryName = categoryName.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
        .replace(/\s+/g, '-') // collapse whitespace and replace by -
        .replace(/-+/g, '-'); // collapse dashes

    document.getElementById('SlugDisplay').value = categoryName;
    document.getElementById('Slug').value = categoryName;
}