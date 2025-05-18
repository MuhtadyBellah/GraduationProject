# 💻 For Developers

{% stepper %}
{% step %}
### Local Setup

{% tabs %}
{% tab title="Flutter" %}
{% content-ref url="code-implementation/flutter-code.md" %}
[flutter-code.md](code-implementation/flutter-code.md)
{% endcontent-ref %}

<pre class="language-bash"><code class="lang-bash"><strong>git clone
</strong>flutter pub get
flutter run
</code></pre>
{% endtab %}

{% tab title="ASP.Net" %}
{% content-ref url="code-implementation/dotnet-code.md" %}
[dotnet-code.md](code-implementation/dotnet-code.md)
{% endcontent-ref %}

```bash
git clone
dotnet restore
dotnet ef database update
dotnet run
```
{% endtab %}

{% tab title="Laravel" %}
{% content-ref url="code-implementation/laravel-code.md" %}
[laravel-code.md](code-implementation/laravel-code.md)
{% endcontent-ref %}

```bash
git clone
composer install
cp .env.example .env
php artisan key:generate
php artisan migrate --seed
php artisan serve
```
{% endtab %}
{% endtabs %}
{% endstep %}

{% step %}
### Running the App

1. Backend: `dotnet run` & `php artisan serve`
2. Flutter: `flutter run` (iOS/Android/Web)
{% endstep %}

{% step %}
### Testing

* Unit tests
* Integration / API tests
{% endstep %}
{% endstepper %}

{% content-ref url="api-endpoints/" %}
[api-endpoints](api-endpoints/)
{% endcontent-ref %}

{% content-ref url="programs-used.md" %}
[programs-used.md](programs-used.md)
{% endcontent-ref %}
