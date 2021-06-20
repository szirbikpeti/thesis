export class FileTableModel {
  public position: number;
  public file: File;
  public preview: string | ArrayBuffer;
  public name: string;
  public format: string;
  public id: string;

  constructor(position: number, file: File, preview: string | ArrayBuffer, id?: string, fileName?: string, format?: string) {
    this.position = position;
    this.file = file;
    this.preview = preview;
    this.id = id;
    this.name = fileName;
    this.format = format;
  }
}
