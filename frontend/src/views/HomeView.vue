<template>
  <div class="min-vh-100 bg-white">
    <!-- نوار بالایی -->
    <nav class="navbar navbar-expand-lg navbar-light bg-white shadow-sm py-3 sticky-top">
      <div class="container">
        <a class="navbar-brand fw-bold text-primary" href="#">
          <i class="bi bi-bar-chart-steps fs-3 me-2"></i>
          <span class="d-none d-sm-inline">TrainMate</span>Pro
        </a>

        <button
          class="navbar-toggler border-0"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarMain"
        >
          <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarMain">
          <ul class="navbar-nav mx-auto mb-2 mb-lg-0">
            <li class="nav-item">
              <a class="nav-link px-3" href="#features">ویژگی‌ها</a>
            </li>
            <li class="nav-item">
              <a class="nav-link px-3" href="#contact">تماس</a>
            </li>
          </ul>

          <div class="d-flex gap-2">
            <router-link to="/login" class="btn btn-outline-primary px-4"> ورود </router-link>
            <router-link to="/register" class="btn btn-primary px-4"> ثبت‌نام </router-link>
          </div>
        </div>
      </div>
    </nav>

    <!-- بخش قهرمان -->
    <section class="py-5 py-md-7 bg-gradient">
      <div class="container">
        <div class="row align-items-center g-5">
          <div class="col-lg-6">
            <span class="badge bg-primary bg-opacity-10 text-primary px-3 py-2 rounded-pill mb-4">
              🏋️ سیستم مدیریت برنامه ورزشی
            </span>

            <h1 class="display-4 fw-bold mb-4">
              مربیگری هوشمند
              <span class="text-primary">همراه شما</span>
            </h1>

            <p class="lead text-secondary mb-5">
              طراحی برنامه تمرینی اختصاصی، مدیریت شاگردان و پیگیری پیشرفت با بانک کامل ۱۵۰۰ حرکت
              ورزشی + تصاویر آموزشی
            </p>
          </div>

          <div class="col-lg-6">
            <div class="position-relative">
              <div class="bg-primary bg-opacity-10 rounded-5 p-4">
                <img
                  src="https://placehold.co/600x400/0d6efd/white?text=Dashboard+Preview"
                  alt="Dashboard Preview"
                  class="img-fluid rounded-4 shadow-lg"
                />
              </div>
              <!-- المان تزئینی -->
              <div class="position-absolute bottom-0 start-0 translate-middle">
                <div class="bg-warning rounded-circle p-4 opacity-25"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ویژگی‌ها -->
    <section id="features" class="py-5 py-md-7 bg-light">
      <div class="container">
        <div class="text-center mb-5">
          <h2 class="display-6 fw-bold mb-3">چرا TrainMatePro</h2>
          <p class="lead text-secondary">ابزارهای حرفه‌ای برای مربیگری مدرن</p>
        </div>

        <div class="row g-4">
          <div class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm rounded-4 p-4">
              <div class="d-flex">
                <div class="bg-primary bg-opacity-10 rounded-3 p-3 me-3">
                  <i class="bi bi-people-fill text-primary fs-3"></i>
                </div>
                <div>
                  <h3 class="h5 fw-bold mb-2">مدیریت حرفه‌ای شاگردان</h3>
                  <p class="text-secondary small mb-0">
                    پروفایل کامل، تاریخچه تمرینات، ثبت رکوردها و آنالیز پیشرفت هر شاگرد
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm rounded-4 p-4">
              <div class="d-flex">
                <div class="bg-success bg-opacity-10 rounded-3 p-3 me-3">
                  <i class="bi bi-calendar-check-fill text-success fs-3"></i>
                </div>
                <div>
                  <h3 class="h5 fw-bold mb-2">طراحی برنامه هوشمند</h3>
                  <p class="text-secondary small mb-0">
                    ساخت برنامه‌های هفتگی با ۱۵۰۰ حرکت، تنظیم ست و تکرار، اضافه کردن ویدیو
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm rounded-4 p-4">
              <div class="d-flex">
                <div class="bg-warning bg-opacity-10 rounded-3 p-3 me-3">
                  <i class="bi bi-phone-fill text-warning fs-3"></i>
                </div>
                <div>
                  <h3 class="h5 fw-bold mb-2">اپلیکیشن شاگرد</h3>
                  <p class="text-secondary small mb-0">
                    شاگردان با موبایل خود برنامه را می‌بینند، تمرینات را ثبت و پیشرفت را چک می‌کنند
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm rounded-4 p-4">
              <div class="d-flex">
                <div class="bg-info bg-opacity-10 rounded-3 p-3 me-3">
                  <i class="bi bi-graph-up-arrow text-info fs-3"></i>
                </div>
                <div>
                  <h3 class="h5 fw-bold mb-2">گزارشات تحلیلی</h3>
                  <p class="text-secondary small mb-0">
                    نمودار پیشرفت، آمار تمرینات، مقایسه دوره‌ها و گزارشات ماهانه
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm rounded-4 p-4">
              <div class="d-flex">
                <div class="bg-danger bg-opacity-10 rounded-3 p-3 me-3">
                  <i class="bi bi-camera-video-fill text-danger fs-3"></i>
                </div>
                <div>
                  <h3 class="h5 fw-bold mb-2">بانک حرکات تصویری</h3>
                  <p class="text-secondary small mb-0">
                    ۱۵۰۰ حرکت با GIF آموزشی، دسته‌بندی بر اساس عضله، وسیله و سطح
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm rounded-4 p-4">
              <div class="d-flex">
                <div class="bg-purple bg-opacity-10 rounded-3 p-3 me-3">
                  <i class="bi bi-chat-dots-fill text-purple fs-3"></i>
                </div>
                <div>
                  <h3 class="h5 fw-bold mb-2">ارتباط مستقیم</h3>
                  <p class="text-secondary small mb-0">
                    چت با شاگردان، ارسال نوتیفیکیشن، یادآوری تمرینات و برنامه‌ها
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- تماس با ما -->
    <section id="contact" class="py-5 py-md-7 bg-light">
      <div class="container">
        <div class="row justify-content-center">
          <div class="col-lg-8 text-center">
            <h2 class="display-6 fw-bold mb-4">آماده شروع هستید؟</h2>

            <div class="d-flex flex-column flex-sm-row gap-3 justify-content-center">
              <router-link to="/register" class="btn btn-primary btn-lg px-5 py-3">
                ثبت‌نام رایگان
              </router-link>
              <a href="#" class="btn btn-outline-secondary btn-lg px-5 py-3">
                <i class="bi bi-whatsapp me-2"></i>
                پشتیبانی واتساپ
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- فوتر -->
    <footer class="py-5 bg-dark text-white">
      <div class="container">
        <div class="row g-4">
          <div class="col-lg-4">
            <a class="navbar-brand fw-bold text-white mb-3 d-inline-block" href="#">
              <i class="bi bi-bar-chart-steps fs-3 me-2"></i>
              TrainMatePro
            </a>
            <p class="text-white-50 small">
              حرفه‌ای ترین سیستم مدیریت تمرینات ورزشی برای مربیان و باشگاه‌ها
            </p>
            <div class="d-flex gap-3">
              <a href="#" class="text-white-50"><i class="bi bi-instagram fs-5"></i></a>
              <a href="#" class="text-white-50"><i class="bi bi-telegram fs-5"></i></a>
              <a href="#" class="text-white-50"><i class="bi bi-whatsapp fs-5"></i></a>
              <a href="#" class="text-white-50"><i class="bi bi-linkedin fs-5"></i></a>
            </div>
          </div>

          <div class="col-lg-2 col-md-4">
            <h5 class="fw-bold mb-3">محصول</h5>
            <ul class="list-unstyled">
              <li class="mb-2">
                <a href="#features" class="text-white-50 text-decoration-none">ویژگی‌ها</a>
              </li>
              <li class="mb-2">
                <a href="#pricing" class="text-white-50 text-decoration-none">تعرفه‌ها</a>
              </li>
              <li class="mb-2">
                <a href="#" class="text-white-50 text-decoration-none">مستندات</a>
              </li>
            </ul>
          </div>

          <div class="col-lg-2 col-md-4">
            <h5 class="fw-bold mb-3">شرکت</h5>
            <ul class="list-unstyled">
              <li class="mb-2">
                <a href="#" class="text-white-50 text-decoration-none">درباره ما</a>
              </li>
              <li class="mb-2"><a href="#" class="text-white-50 text-decoration-none">وبلاگ</a></li>
              <li class="mb-2">
                <a href="#contact" class="text-white-50 text-decoration-none">تماس</a>
              </li>
            </ul>
          </div>

          <div class="col-lg-4 col-md-4">
            <h5 class="fw-bold mb-3">خبرنامه</h5>
            <p class="text-white-50 small">آخرین اخبار و آموزش‌ها را دریافت کنید</p>
            <div class="input-group">
              <input type="email" class="form-control" placeholder="ایمیل شما" />
              <button class="btn btn-primary" type="button">عضویت</button>
            </div>
          </div>
        </div>

        <hr class="my-4 bg-white-50" />

        <div class="text-center text-white-50 small">
          <p class="mb-0">© ۲۰۲۶ TrainMatePro. تمامی حقوق محفوظ است.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
// فقط روت‌ها - دیگه دمو نداریم
</script>

<style scoped>
.bg-gradient {
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
}

.text-purple {
  color: #6f42c1;
}

.bg-purple {
  background-color: #6f42c1;
}

.py-md-7 {
  padding-top: 5rem;
  padding-bottom: 5rem;
}

@media (max-width: 768px) {
  .py-md-7 {
    padding-top: 3rem;
    padding-bottom: 3rem;
  }
}
</style>
