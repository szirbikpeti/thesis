export function isNull(value: any) {
  return value === null || value === undefined || value === '';
}

export function getPicture(picture: string) {
  const objectURL = 'data:image/jpeg;base64,' + picture;

  return this.sanitizer.bypassSecurityTrustUrl(objectURL);
}
