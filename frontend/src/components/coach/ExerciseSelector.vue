<template>
  <div>
    <!-- فیلترهای سریع -->
    <div class="filter-tabs mb-3 d-flex flex-wrap gap-2">
      <button
        v-for="filter in quickFilters"
        :key="filter.value"
        class="btn btn-sm"
        :class="selectedQuickFilter === filter.value ? 'btn-primary' : 'btn-outline-secondary'"
        @click="setQuickFilter(filter.value)"
        type="button"
      >
        {{ filter.label }}
      </button>
    </div>

    <!-- دسته‌بندی اصلی -->
    <div class="category-tabs mb-3 d-flex gap-2">
      <button
        v-for="cat in categories"
        :key="cat.value"
        class="btn btn-sm flex-fill"
        :class="selectedCategory === cat.value ? 'btn-primary' : 'btn-outline-secondary'"
        @click="setCategory(cat.value)"
        type="button"
      >
        {{ cat.label }}
      </button>
    </div>

    <!-- زیرمجموعه‌ها -->
    <div class="subcategory-section mb-3" v-if="subcategories.length > 0">
      <div class="d-flex flex-wrap gap-2">
        <button
          v-for="sub in subcategories"
          :key="sub"
          class="btn btn-sm"
          :class="selectedSubcategory === sub ? 'btn-success' : 'btn-outline-success'"
          @click="setSubcategory(sub)"
          type="button"
        >
          {{ sub }}
        </button>
      </div>
    </div>

    <!-- جستجو -->
    <div class="search-container mb-3">
      <div class="input-group">
        <span class="input-group-text bg-white border-end-0">
          <i class="bi bi-search"></i>
        </span>
        <input
          type="text"
          class="form-control border-start-0"
          v-model="searchQuery"
          placeholder="جستجوی حرکت..."
          @input="debouncedSearch"
        />
        <button
          v-if="searchQuery"
          class="btn btn-outline-secondary"
          type="button"
          @click="clearSearch"
        >
          <i class="bi bi-x-lg"></i>
        </button>
      </div>
    </div>

    <!-- نتایج -->
    <div class="results-section" style="max-height: 400px; overflow-y: auto">
      <!-- Loading -->
      <div v-if="loading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status"></div>
        <p class="text-secondary mt-3">در حال بارگذاری حرکات...</p>
      </div>

      <!-- نتایج -->
      <div v-else-if="filteredExercises.length > 0" class="list-group">
        <button
          v-for="exercise in filteredExercises"
          :key="exercise.exerciseId"
          class="list-group-item list-group-item-action d-flex gap-3 align-items-center"
          @click="selectExercise(exercise)"
          type="button"
        >
          <!-- GIF -->
          <div style="width: 50px; height: 50px; flex-shrink: 0">
            <img
              :src="getGifUrl(exercise.gifUrl)"
              :alt="exercise.name"
              class="w-100 h-100 object-fit-cover rounded"
              loading="lazy"
              @error="handleImageError"
            />
          </div>

          <!-- اطلاعات -->
          <div class="flex-grow-1 text-start">
            <h6 class="mb-1">{{ exercise.name }}</h6>
            <div class="d-flex gap-2 small">
              <span class="badge bg-light text-dark">
                <i class="bi bi-activity me-1"></i>
                {{ getTargetMuscle(exercise) }}
              </span>
              <span class="badge bg-light text-dark">
                <i class="bi bi-tools me-1"></i>
                {{ getEquipment(exercise) }}
              </span>
            </div>
          </div>

          <i class="bi bi-chevron-left text-secondary"></i>
        </button>
      </div>

      <!-- بدون نتیجه -->
      <div v-else class="text-center py-5">
        <i class="bi bi-emoji-frown fs-1 text-secondary opacity-25"></i>
        <p class="text-secondary mt-3">حرکتی یافت نشد</p>
        <button class="btn btn-sm btn-outline-primary" @click="resetFilters">
          <i class="bi bi-arrow-repeat me-1"></i>
          پاک کردن فیلترها
        </button>
      </div>
    </div>

    <!-- دکمه بستن -->
    <div class="mt-3 d-flex justify-content-end">
      <button type="button" class="btn btn-secondary" @click="$emit('close')">انصراف</button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { exerciseService } from '../../services/exercises'

const emit = defineEmits(['select', 'close'])

// ========== STATE ==========
const loading = ref(false)
const allExercises = ref([])
const filteredExercises = ref([])
const searchQuery = ref('')
const selectedQuickFilter = ref('all')
const selectedCategory = ref('all')
const selectedSubcategory = ref('')

let searchTimer = null

// ========== FILTERS ==========
const quickFilters = [
  { value: 'all', label: 'همه' },
  { value: 'chest', label: 'سینه' },
  { value: 'back', label: 'پشت' },
  { value: 'legs', label: 'پا' },
  { value: 'shoulders', label: 'سرشانه' },
  { value: 'arms', label: 'بازو' },
  { value: 'abs', label: 'شکم' },
]

const categories = [
  { value: 'all', label: 'همه' },
  { value: 'muscle', label: 'عضله هدف' },
  { value: 'bodypart', label: 'بخش بدن' },
  { value: 'equipment', label: 'وسیله' },
]

// زیرمجموعه‌ها
const subcategories = computed(() => {
  if (selectedCategory.value === 'muscle') {
    return [
      ...new Set(
        allExercises.value.flatMap((ex) => ex.targetMuscles?.map((m) => translateMuscle(m)) || []),
      ),
    ].sort()
  } else if (selectedCategory.value === 'bodypart') {
    return [
      ...new Set(
        allExercises.value.flatMap((ex) => ex.bodyParts?.map((b) => translateBodyPart(b)) || []),
      ),
    ].sort()
  } else if (selectedCategory.value === 'equipment') {
    return [
      ...new Set(
        allExercises.value.flatMap((ex) => ex.equipments?.map((e) => translateEquipment(e)) || []),
      ),
    ].sort()
  }
  return []
})

// ========== توابع ترجمه ==========
const translateMuscle = (muscle) => {
  const translations = {
    abs: 'شکم',
    pectorals: 'سینه',
    chest: 'سینه',
    lats: 'پشت',
    traps: 'ذوزنقه',
    delts: 'سرشانه',
    shoulders: 'سرشانه',
    biceps: 'جلو بازو',
    triceps: 'پشت بازو',
    forearms: 'ساعد',
    quads: 'جلو پا',
    hamstrings: 'پشت پا',
    glutes: 'باسن',
    calves: 'ساق پا',
    'upper back': 'پشت',
    'upper arms': 'بازو',
    'lower arms': 'ساعد',
    'upper legs': 'پا',
    'lower legs': 'ساق پا',
    waist: 'شکم',
    back: 'پشت',
  }
  return translations[muscle] || muscle
}

const translateEquipment = (equipment) => {
  const translations = {
    'body weight': 'بدون وسیله',
    dumbbell: 'دمبل',
    barbell: 'هالتر',
    cable: 'سیم‌کش',
    band: 'کش',
    kettlebell: 'کتل بل',
    machine: 'دستگاه',
    'smith machine': 'اسمیت',
    'ez barbell': 'هالتر EZ',
    'trap bar': 'هالتر ذوزنقه',
    'medicine ball': 'توپ طبی',
    'stability ball': 'توپ تعادلی',
    rope: 'طناب',
    roller: 'غلتک',
  }
  return translations[equipment] || equipment
}

const translateBodyPart = (part) => {
  const translations = {
    chest: 'سینه',
    back: 'پشت',
    shoulders: 'سرشانه',
    'upper arms': 'بازو',
    'lower arms': 'ساعد',
    'upper legs': 'پا',
    'lower legs': 'ساق پا',
    waist: 'شکم',
    neck: 'گردن',
    cardio: 'قلبی عروقی',
  }
  return translations[part] || part
}

// ========== توابع کمکی برای نمایش ==========
const getTargetMuscle = (exercise) => {
  if (exercise.targetMuscles?.length) {
    return translateMuscle(exercise.targetMuscles[0])
  }
  return 'عضله'
}

const getEquipment = (exercise) => {
  if (exercise.equipments?.length) {
    return translateEquipment(exercise.equipments[0])
  }
  return 'وسیله'
}
const getGifUrl = (url) => {
  const fileName = url.split('/').pop()
  return `/media/exercises/${fileName}`
}
const handleImageError = (e) => {
  // اگه عکس لود نشد، از آدرس اصلی استفاده کن
  const originalUrl = e.target.getAttribute('data-original') || e.target.src

  if (originalUrl.includes('localhost') || originalUrl.includes('127.0.0.1')) {
    // اگه آدرس لوکال بود و کار نکرد، برو سراغ آدرس اصلی
    const fileName = originalUrl.split('/').pop()
    e.target.src = `https://static.exercisedb.dev/media/${fileName}`
  } else {
    // آدرس پیش‌فرض
    e.target.src = 'https://via.placeholder.com/60?text=No+Image'
  }
}

// ========== توابع فیلتر ==========
const setQuickFilter = (filter) => {
  selectedQuickFilter.value = filter
  selectedCategory.value = 'all'
  selectedSubcategory.value = ''
  filterExercises()
}

const setCategory = (category) => {
  selectedCategory.value = category
  selectedSubcategory.value = ''
  filterExercises()
}

const setSubcategory = (sub) => {
  selectedSubcategory.value = sub
  selectedQuickFilter.value = 'all'
  filterExercises()
}

const resetFilters = () => {
  selectedQuickFilter.value = 'all'
  selectedCategory.value = 'all'
  selectedSubcategory.value = ''
  searchQuery.value = ''
  filterExercises()
}

const clearSearch = () => {
  searchQuery.value = ''
  filterExercises()
}

const filterExercises = () => {
  let filtered = [...allExercises.value]

  // فیلتر بر اساس جستجو
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(
      (ex) =>
        ex.name?.toLowerCase().includes(query) ||
        ex.targetMuscles?.some((m) => translateMuscle(m).toLowerCase().includes(query)) ||
        ex.equipments?.some((e) => translateEquipment(e).toLowerCase().includes(query)),
    )
  }

  // فیلتر بر اساس دسته سریع
  if (selectedQuickFilter.value !== 'all') {
    const filterMap = {
      chest: ['سینه', 'chest', 'pectorals'],
      back: ['پشت', 'back', 'lats'],
      legs: ['پا', 'leg', 'quads', 'hamstrings', 'glutes', 'calves'],
      shoulders: ['سرشانه', 'shoulder', 'delts'],
      arms: ['بازو', 'arm', 'biceps', 'triceps', 'forearms'],
      abs: ['شکم', 'abs', 'waist'],
    }

    filtered = filtered.filter((ex) => {
      const targets = filterMap[selectedQuickFilter.value] || []
      return ex.targetMuscles?.some((m) =>
        targets.some((t) => m.toLowerCase().includes(t) || translateMuscle(m).includes(t)),
      )
    })
  }

  // فیلتر بر اساس دسته‌بندی
  if (selectedCategory.value !== 'all' && selectedSubcategory.value) {
    filtered = filtered.filter((ex) => {
      if (selectedCategory.value === 'muscle') {
        return ex.targetMuscles?.some((m) => translateMuscle(m).includes(selectedSubcategory.value))
      } else if (selectedCategory.value === 'bodypart') {
        return ex.bodyParts?.some((b) => translateBodyPart(b).includes(selectedSubcategory.value))
      } else if (selectedCategory.value === 'equipment') {
        return ex.equipments?.some((e) => translateEquipment(e).includes(selectedSubcategory.value))
      }
      return true
    })
  }

  filteredExercises.value = filtered
}

const debouncedSearch = () => {
  if (searchTimer) clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    filterExercises()
  }, 300)
}

// ========== بارگذاری حرکات ==========
const loadExercises = async () => {
  loading.value = true
  try {
    const response = await exerciseService.getExercises({ limit: 200 })
    if (response.success) {
      allExercises.value = response.exercises || []
      filteredExercises.value = allExercises.value
    }
  } catch (err) {
    console.error('Error loading exercises:', err)
  } finally {
    loading.value = false
  }
}

// ========== انتخاب حرکت ==========
const selectExercise = (exercise) => {
  emit('select', exercise)
}

// ========== متد عمومی برای ریست از بیرون ==========
defineExpose({
  resetFilters,
})

// ========== Lifecycle ==========
loadExercises()

// واچ برای فیلتر کردن مجدد
watch([selectedQuickFilter, selectedCategory, selectedSubcategory], () => {
  filterExercises()
})
</script>

<style scoped>
.object-fit-cover {
  object-fit: cover;
}

.filter-tabs,
.category-tabs {
  overflow-x: auto;
  padding-bottom: 0.5rem;
  white-space: nowrap;
}

.filter-tabs button,
.category-tabs button {
  white-space: nowrap;
}

.results-section::-webkit-scrollbar {
  width: 6px;
}

.results-section::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.results-section::-webkit-scrollbar-thumb {
  background: #ccc;
  border-radius: 10px;
}

.results-section::-webkit-scrollbar-thumb:hover {
  background: #999;
}
</style>
