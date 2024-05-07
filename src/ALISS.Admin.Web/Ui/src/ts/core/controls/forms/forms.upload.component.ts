class Upload {
	constructor() {

		enum CLASSES {
			upload = '.aliss-form__file-btn'
		}

		enum ID {
			OrganisationId = "OrganisationId",
			ServiceId = "ServiceId",
			OrganisationLogo = 'OrganisationLogo',
			ServiceLogo = 'ServiceLogo',
			Gallery1 = 'Gallery1',
			Gallery2 = 'Gallery2',
			Gallery3 = 'Gallery3',
			ServiceGalleryImage1 = 'ServiceGalleryImage1',
			ServiceGalleryImage2 = 'ServiceGalleryImage2',
			ServiceGalleryImage3 = 'ServiceGalleryImage3',
			ServiceGalleryImageId1 = 'ServiceGalleryImageId1',
			ServiceGalleryImageId2 = 'ServiceGalleryImageId2',
			ServiceGalleryImageId3 = 'ServiceGalleryImageId3',
			OrganisationLogoRemove = 'OrganisationLogoRemove',
			ServiceLogoRemove = 'ServiceLogoRemove',
			Gallery1Remove = 'Gallery1Remove',
			Gallery2Remove = 'Gallery2Remove',
			Gallery3Remove = 'Gallery3Remove',
			Gallery1AltText = 'ServiceGallery1AltText',
			Gallery2AltText = 'ServiceGallery2AltText',
			Gallery3AltText = 'ServiceGallery3AltText',
			OrganisationLogoUploaded = 'OrganisationLogoUploaded',
			ServiceLogoUploaded = 'ServiceLogoUploaded',
			GalleryUploaded = 'GalleryUploaded',
			OrganisationLogoDisplay = 'OrganisationLogoDisplay',
			ServiceLogoDisplay = 'ServiceLogoDisplay',
			Media1Display = 'Media1Display',
			Media2Display = 'Media2Display',
			Media3Display = 'Media3Display',
			UserId = 'UserId',
			IsAdmin = 'IsAdmin'
		}

		const init = () => {
			const buttons = document.querySelectorAll(CLASSES.upload);
			for (const button of buttons) {
				const actualButton = button.nextElementSibling as HTMLInputElement;
				const fileChosen = button.nextElementSibling?.nextElementSibling as HTMLDivElement;
				button.addEventListener("click", function(e){
					e.preventDefault();
					actualButton.click();
				});
				actualButton.addEventListener('change', function(){
					var imageType = this.id;
					var image = this.files instanceof FileList ? this.files[0] : null;
					var mediaGalleryId = "";
					var mediaGalleryUrl = "";
					var mediaRef = 0;
					if (imageType == "Gallery1") {
						mediaGalleryId = (document.getElementById(ID.ServiceGalleryImageId1) as HTMLInputElement).value;
						mediaGalleryUrl = (document.getElementById(ID.Media1Display)?.children[1] as HTMLImageElement).src;
						mediaRef = 1;
					}
					else if (imageType == "Gallery2") {
						mediaGalleryId = (document.getElementById(ID.ServiceGalleryImageId2) as HTMLInputElement).value;
						mediaGalleryUrl = (document.getElementById(ID.Media2Display)?.children[1] as HTMLImageElement).src;
						mediaRef = 2;
					}
					else if (imageType == "Gallery3") {
						mediaGalleryId = (document.getElementById(ID.ServiceGalleryImageId3) as HTMLInputElement).value;
						mediaGalleryUrl = (document.getElementById(ID.Media3Display)?.children[1] as HTMLImageElement).src;
						mediaRef = 3;
					}

					if (image) {
						var orgId = (document.getElementById(ID.OrganisationId) as HTMLInputElement).value;
						var serId = (document.getElementById(ID.ServiceId) as HTMLInputElement).value;
						var userId = (document.getElementById(ID.UserId) as HTMLInputElement).value;
						var isAdmin = (document.getElementById(ID.IsAdmin) as HTMLInputElement).value;

						var reader = new FileReader();
						var val;
						reader.onload = function (e) {
							val = e.target?.result;
							var url = "/api/mediagallery/UploadImage";
							var data = {
								"organisationId": orgId,
								"serviceId": serId,
								"mediaGalleryId": mediaGalleryId,
								"mediaGalleryUrl": mediaGalleryUrl,
								"imageType": imageType,
								"imageName": image?.name,
								"imageData": val,
								"userId": userId,
								"isAdmin": isAdmin,
								"galleryRef": mediaRef
							};
							let params: RequestInit = {
								method: "POST",
								body: JSON.stringify(data),
								headers: { "Content-Type": "application/json"}
							}
							fetch(
								url, 
								params
							)
								.then((result) => result.json())
								.then((resultJson) => {
									var stringResult = resultJson.url as string;
									if (stringResult && stringResult.startsWith("http")) {
										if (imageType == "OrganisationLogo") {
											let imageDisplay = document.getElementById(ID.OrganisationLogoDisplay);
											imageDisplay?.children[1].setAttribute("src", stringResult + "#" + new Date().getTime());
											imageDisplay?.removeAttribute("hidden");
											var orgUploaded = document.getElementById(ID.OrganisationLogoUploaded) as HTMLHeadingElement;
											orgUploaded.textContent = "1 of 1 uploaded";
										}
										else if (imageType == "ServiceLogo") {
											let imageDisplay = document.getElementById(ID.ServiceLogoDisplay);
											imageDisplay?.children[1].setAttribute("src", stringResult + "#" + new Date().getTime());
											imageDisplay?.removeAttribute("hidden");
											var serUploaded = document.getElementById(ID.ServiceLogoUploaded) as HTMLHeadingElement;
											serUploaded.textContent = "1 of 1 uploaded";
										}
										else if (imageType == "Gallery1") {
											let id = document.getElementById(ID.ServiceGalleryImageId1) as HTMLInputElement;
											id.value = resultJson.galleryId;
											let imageDisplay = document.getElementById(ID.Media1Display);
											imageDisplay?.children[1].setAttribute("src", stringResult + "#" + new Date().getTime());
											imageDisplay?.removeAttribute("hidden");
											let galUploaded = document.getElementById(ID.GalleryUploaded) as HTMLHeadingElement;
											if (mediaGalleryUrl != "" || mediaGalleryUrl != null) {
												galUploaded.textContent = Math.min(3, Number(galUploaded.textContent?.charAt(0)) + 1) + " of 3 uploaded";
										}
										}
										else if (imageType == "Gallery2") {
											let id = document.getElementById(ID.ServiceGalleryImageId2) as HTMLInputElement;
											id.value = resultJson.galleryId;
											let imageDisplay = document.getElementById(ID.Media2Display);
											imageDisplay?.children[1].setAttribute("src", stringResult + "#" + new Date().getTime());
											imageDisplay?.removeAttribute("hidden");
											let galUploaded = document.getElementById(ID.GalleryUploaded) as HTMLHeadingElement;
											if (mediaGalleryUrl != "" || mediaGalleryUrl != null) {
												galUploaded.textContent = Math.min(3, Number(galUploaded.textContent?.charAt(0)) + 1) + " of 3 uploaded";
										}
										}
										else if (imageType == "Gallery3") {
											let id = document.getElementById(ID.ServiceGalleryImageId3) as HTMLInputElement;
											id.value = resultJson.galleryId;
											let imageDisplay = document.getElementById(ID.Media3Display);
											imageDisplay?.children[1].setAttribute("src", stringResult + "#" + new Date().getTime());
											imageDisplay?.removeAttribute("hidden");
											let galUploaded = document.getElementById(ID.GalleryUploaded) as HTMLHeadingElement;
											if (mediaGalleryUrl != "" || mediaGalleryUrl != null) {
												galUploaded.textContent = Math.min(3, Number(galUploaded.textContent?.charAt(0)) + 1) + " of 3 uploaded";
										}
									}
									}
									else {
										let mediaErrors = document.getElementById('media-errors') as HTMLDivElement;
										
										if(mediaErrors){
											let errorDiv = document.querySelector('.validation-summary-errors') as HTMLDivElement;
											if(!errorDiv){
												errorDiv = document.querySelector('.validation-summary-valid') as HTMLDivElement;
											}
											if(errorDiv){
												mediaErrors.hidden = false;

												let errorList = errorDiv.children.item(0) as HTMLUListElement;
												var error = document.createElement("li");
												error.appendChild(document.createTextNode(resultJson));
												errorList.appendChild(error);

												switch(imageType){
													case 'OrganisationLogo':
														break;
													case 'ServiceLogo':
														break;
													case 'Gallery1':
														break;
													case 'Gallery2':
														break;
													case 'Gallery3':
														break;
												}
											}								
										}
									}
								});
						}
						reader.readAsDataURL(image);
					}
					// fileChosen.textContent = file;
				});
			}

			var orgLogoRemove = document.getElementById(ID.OrganisationLogoRemove) as HTMLButtonElement;

			var serLogoRemove = document.getElementById(ID.ServiceLogoRemove) as HTMLButtonElement;

			var gal1Remove = document.getElementById(ID.Gallery1Remove) as HTMLButtonElement;
			var gal1Alt = document.getElementById(ID.Gallery1AltText) as HTMLInputElement;
			var gal1AltCount = gal1Alt?.nextElementSibling as HTMLSpanElement;

			var gal2Remove = document.getElementById(ID.Gallery2Remove) as HTMLButtonElement;
			var gal2Alt = document.getElementById(ID.Gallery2AltText) as HTMLInputElement;
			var gal2AltCount = gal2Alt?.nextElementSibling as HTMLSpanElement;

			var gal3Remove = document.getElementById(ID.Gallery3Remove) as HTMLButtonElement;
			var gal3Alt = document.getElementById(ID.Gallery3AltText) as HTMLInputElement;
			var gal3AltCount = gal3Alt?.nextElementSibling as HTMLSpanElement;

			orgLogoRemove?.addEventListener("click", function(e) {
				e.preventDefault();
				var orgLogo = document.getElementsByName(ID.OrganisationLogo) as NodeListOf<HTMLInputElement>;
				var orgUploaded = document.getElementById(ID.OrganisationLogoUploaded) as HTMLHeadingElement;
				for (var input of orgLogo) {
					input.value = "";
				}
				orgUploaded.textContent = "0 of 1 uploaded";
				
				let imageDisplay = document.getElementById(ID.OrganisationLogoDisplay);
				imageDisplay?.setAttribute("hidden", "true");
				var orgId = (document.getElementById(ID.OrganisationId) as HTMLInputElement).value;
				var url = "/api/mediagallery/RemoveImage";
				var data = {
					"organisationId": orgId
				};
				let params: RequestInit = {
					method: "POST",
					body: JSON.stringify(data),
					headers: { "Content-Type": "application/json"}
				}
				fetch(
					url, 
					params
				)
					.then((result) => result.json())
					.then((resultJson) => {});
					});

			serLogoRemove?.addEventListener("click", function(e) {
				e.preventDefault();
				var serLogo = document.getElementsByName(ID.ServiceLogo) as NodeListOf<HTMLInputElement>;
				var serUploaded = document.getElementById(ID.ServiceLogoUploaded) as HTMLHeadingElement;
				for (var input of serLogo) {
					input.value = "";
				}
				serUploaded.textContent = "0 of 1 uploaded"
				
				let imageDisplay = document.getElementById(ID.ServiceLogoDisplay);
				imageDisplay?.setAttribute("hidden", "true");
				var serId = (document.getElementById(ID.ServiceId) as HTMLInputElement).value;
				var url = "/api/mediagallery/RemoveImage";
				var data = {
					"serviceId": serId
				};
				let params: RequestInit = {
					method: "POST",
					body: JSON.stringify(data),
					headers: { "Content-Type": "application/json"}
				}
				fetch(
					url, 
					params
				)
					.then((result) => result.json())
					.then((resultJson) => {});
			});

			gal1Remove?.addEventListener("click", function(e) {
				e.preventDefault();
				var gal1Input = document.getElementById(ID.Gallery1) as HTMLInputElement;
				var gal1Hidden = document.getElementById(ID.ServiceGalleryImage1) as HTMLInputElement;
				var galUploaded = document.getElementById(ID.GalleryUploaded) as HTMLHeadingElement;
				var gal1Id = document.getElementById(ID.ServiceGalleryImageId1) as HTMLInputElement;

				gal1Input.value = "";
				gal1Hidden.value = "";
				gal1Alt.value = "";
				gal1AltCount.textContent = "140 character(s) remaining";
				galUploaded.textContent = Math.max(Number(galUploaded.textContent?.charAt(0)) - 1, 0) + " of 3 uploaded";
				
				let imageDisplay = document.getElementById(ID.Media1Display) as HTMLDivElement;
				imageDisplay?.setAttribute("hidden", "true");
				let mediaGalleryId = gal1Id.value;
				let mediaGalleryUrl = (imageDisplay.children[1] as HTMLImageElement).src;
				gal1Id.value = "00000000-0000-0000-0000-000000000000";
				var url = "/api/mediagallery/RemoveImage";
				var data = {
					"MediaGalleryId": mediaGalleryId,
					"MediaGalleryUrl": mediaGalleryUrl.trim()
				};
				let params: RequestInit = {
					method: "POST",
					body: JSON.stringify(data),
					headers: { "Content-Type": "application/json"}
				}
				fetch(
					url, 
					params
				)
					.then((result) => result.json())
					.then((resultJson) => {
					});
			});

			gal2Remove?.addEventListener("click", function(e) {
				e.preventDefault();
				var gal2Input = document.getElementById(ID.Gallery2) as HTMLInputElement;
				var gal2Hidden = document.getElementById(ID.ServiceGalleryImage2) as HTMLInputElement;
				var galUploaded = document.getElementById(ID.GalleryUploaded) as HTMLHeadingElement;
				var gal2Id = document.getElementById(ID.ServiceGalleryImageId2) as HTMLInputElement;
				gal2Input.value = "";
				gal2Hidden.value = "";
				gal2Alt.value = "";
				gal2AltCount.textContent = "140 character(s) remaining";
				galUploaded.textContent = Math.max(Number(galUploaded.textContent?.charAt(0)) - 1, 0) + " of 3 uploaded";
				
				let imageDisplay = document.getElementById(ID.Media2Display) as HTMLDivElement;
				imageDisplay?.setAttribute("hidden", "true");
				let mediaGalleryId = gal2Id.value;
				let mediaGalleryUrl = (imageDisplay.children[1] as HTMLImageElement).src;
				gal2Id.value = "00000000-0000-0000-0000-000000000000";
				var url = "/api/mediagallery/RemoveImage";
				var data = {
					"MediaGalleryId": mediaGalleryId,
					"MediaGalleryUrl": mediaGalleryUrl
				};
				let params: RequestInit = {
					method: "POST",
					body: JSON.stringify(data),
					headers: { "Content-Type": "application/json"}
				}
				fetch(
					url, 
					params
				)
					.then((result) => result.json())
					.then((resultJson) => {
					});
			});

			gal3Remove?.addEventListener("click", function(e) {
				e.preventDefault();
				var gal3Input = document.getElementById(ID.Gallery3) as HTMLInputElement;
				var gal3Hidden = document.getElementById(ID.ServiceGalleryImage3) as HTMLInputElement;
				var galUploaded = document.getElementById(ID.GalleryUploaded) as HTMLHeadingElement;
				var gal3Id = document.getElementById(ID.ServiceGalleryImageId3) as HTMLInputElement;
				gal3Input.value = "";
				gal3Hidden.value = "";
				gal3Alt.value = "";
				gal3AltCount.textContent = "140 character(s) remaining";
				galUploaded.textContent = Math.max(Number(galUploaded.textContent?.charAt(0)) - 1, 0) + " of 3 uploaded";
				
				let imageDisplay = document.getElementById(ID.Media3Display) as HTMLDivElement;
				imageDisplay?.setAttribute("hidden", "true");
				let mediaGalleryId = gal3Id.value;
				let mediaGalleryUrl = (imageDisplay.children[1] as HTMLImageElement).src;
				gal3Id.value = "00000000-0000-0000-0000-000000000000";				
				var url = "/api/mediagallery/RemoveImage";
				var data = {
					"MediaGalleryId": mediaGalleryId,
					"MediaGalleryUrl": mediaGalleryUrl
				};
				let params: RequestInit = {
					method: "POST",
					body: JSON.stringify(data),
					headers: { "Content-Type": "application/json"}
				}
				fetch(
					url, 
					params
				)
					.then((result) => result.json())
					.then((resultJson) => {
					});
			});
		}
		init();
	}
}

export default Upload;