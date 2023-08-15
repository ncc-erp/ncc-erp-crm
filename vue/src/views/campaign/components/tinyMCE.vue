<template>
  <div class="container">
    <editor
      v-model="editorContent"
      api-key="i68nshmn2313ig8km52u3b20azi6i5ufdmx7dmnt25jfrrj4"
      :init="{
         height: 400,
         menubar: true,
         plugins: [
           'advlist autolink lists link image charmap print preview anchor',
           'searchreplace visualblocks code fullscreen',
           'insertdatetime media table paste code help wordcount pageembed'
         ],
         toolbar:
           'undo redo | formatselect | fontsizeselect | bold italic underline backcolor forecolor| \
           alignleft aligncenter alignright alignjustify | \
           bullist numlist outdent indent | removeformat | link image pageembed code | help',
          images_upload_handler: (blobInfo, success, failure, progress) => {
            handleUploadImg(blobInfo, success, failure, progress)
          },
          images_upload_url: 'postAcceptor.ts',
          extended_valid_elements : 'iframe[src|frameborder|style|scrolling|class|width|height|name|align]'
       }"
      @onChange="emitValue"
    />
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Emit, Prop, Watch } from 'vue-property-decorator'
import editor from '@tinymce/tinymce-vue'
@Component({
  components: {
    editor
  }
})
export default class extends Vue {
  @Prop() value: string
  private editorContent: any = ''

  created() {
      this.editorContent = this.value
  }

  @Watch('value')
  setContent() {
    this.editorContent = this.value
  }

  @Emit('onChange') emitValue() {
    return this.editorContent
  }

  async handleUploadImg(blobInfo, success, failure, progress) {
    let formData = new FormData();
    formData.append('file', blobInfo.blob(), blobInfo.filename());
    let response = await this.$store.dispatch({
      type: 'tinymce/uploadImage',
      data: formData
    })
  }

}
</script>
<style scoped>
</style>

