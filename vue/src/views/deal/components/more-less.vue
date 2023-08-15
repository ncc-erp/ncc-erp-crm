<template>
    <p v-if="value.length <= limit"> <b v-if="name!== undefined">@{{ name + " " }}</b>{{ value }}</p>
  <p v-else-if="value.length > limit && !showMore"><b v-if="name!== undefined">@{{ name + " " }}</b>{{ value.substring(0, limit) }}...
        <a class="hover" @click="onclick()">show more</a>
    </p>
    <p v-else-if="value.length > limit && showMore"><b v-if="name!== undefined">@{{ name + " " }}</b>{{ value }}
        <a class="hover" @click="onclick()">show less</a>
    </p>
</template>
<script lang="ts">
import { Vue, Prop, Component } from 'vue-property-decorator'
@Component({
    name: 'MoreLess'
})
export default class MoreLess extends Vue {
    @Prop() value: string
    @Prop() limit: number
    @Prop() name: string
    showMore: boolean = false;
    created() {
        this.showMore = false;
    }
    onclick() {
        this.showMore = !this.showMore;
    }
}
</script>
<style lang="css">
.hover{
    color: #2d8cf0;
    background: 0 0;
    text-decoration: none;
    outline: 0;
    cursor: pointer;
    transition: color .2s ease;
}
.hover:hover{
    text-decoration: underline;
}
b{
  color: #2b85e4;
}
</style>