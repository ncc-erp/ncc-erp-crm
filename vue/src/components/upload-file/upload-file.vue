<template>
  <div class="upload-btn-wrapper">
    <Icon type="md-cloud-upload" size="25" /> 
    <!-- <Button class="btn-edit">Upload a file</Button> -->
    <input
      id="fileUpload"
      type="file"
      @change="handleInputFile"
      accept="image/*, application/pdf, .csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel, application/msword"
    />
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Emit } from "vue-property-decorator";
import { FileType } from "../../store/entities/file-type";
import AbpBase from "@/lib/abpbase";
import { EntityDefault } from "@/store/entities/entity";
@Component({
  props: {
    canSubmit: Boolean,
  },
})
export default class UploadFile extends AbpBase {
  @Prop() entity: Number;
  @Prop() entityId: Number;
  @Emit("handleInputFile") async handleInputFile(e: any) {
    let files = e.target.files || e.dataTransfer.files;
    if (!files.length) {
      return;
    }
      if(this.entity == EntityDefault.Invoice){
        let params = {
          invoiceId: this.entityId,
          file: e.target.files[0],
          type: this.getFileType(files.item(0).name)
        };
        let formData = new FormData();
        for(let key in params){
          formData.append(key, params[key]);
        }
        let response = await this.$store.dispatch({
          type: 'invoice/addFile',
          data: formData
        });
        return response
      }
      if(this.entity == EntityDefault.Deal){
        let params = {
          dealId: this.entityId,
          file: e.target.files[0],
          type: this.getFileType(files.item(0).name)
        };
        let formData = new FormData();
        for(let key in params){
          formData.append(key, params[key]);
        }
        let response = await this.$store.dispatch({
          type: 'deal/addFile',
          data: formData
        });
        return response
      }
      if(this.entity == EntityDefault.Contract){
        let params = {
          contractId: this.entityId,
          file: e.target.files[0],
        };
        let formData = new FormData();
        for(let key in params){
          formData.append(key, params[key]);
        }
        let response = await this.$store.dispatch({
          type: 'contract/addFile',
          data: formData
        });
        return response
      }
  }

  getFileType(fileName: string): FileType {
    let fileType;
    let fileExtension = /[.]/.exec(fileName)
      ? /[^.]+$/.exec(fileName)[0]
      : undefined;
    switch (fileExtension) {
      case "xlsx":
        fileType = FileType.Excel;
        break;
      case "xls":
        fileType = FileType.Excel;
        break;
      case "csv":
        fileType = FileType.Excel;
        break;
      case "doc":
        fileType = FileType.Word;
        break;
      case "docx":
        fileType = FileType.Word;
        break;
      case "pdf":
        fileType = FileType.PDF;
        break;
      case "jpeg":
        fileType = FileType.Image;
        break;
      case "png":
        fileType = FileType.Image;
        break;
      case "jpg":
        fileType = FileType.Image;
        break;
    }
    return fileType;
  }
}
</script>
<style lang="scss">
  .upload-btn-wrapper {
    position: relative;
    overflow: hidden;
    display: inline-block;
  }

  .upload-btn-wrapper input[type=file] {
    font-size: 100px;
    position: absolute;
    left: 0;
    top: 0;
    opacity: 0;
  }
</style>