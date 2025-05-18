# 💡 What we do

## Improved Customer Experience

{% stepper %}
{% step %}
### **User-Friendly Interface**

* **Design intuitive navigation using modern UI/UX principles. Employ frameworks like Flutter to create a consistent look across platforms.**
* **Perform usability testing to gather feedback from real users.**
* **Prioritize responsiveness for seamless experience across devices.**
{% endstep %}

{% step %}
### S**ecure Payment Options**

* **Integrate trusted payment gateways like Stripe, PayPal, or Paybom, FawryPay. Ensure PCI-DSS compliance for handling cardholder data securely.**
{% endstep %}
{% endstepper %}

***

## Personal Data Protection

{% stepper %}
{% step %}
### Encryption Password
{% endstep %}

{% step %}
### S**ecure Authentication and Authorization**

* **Use tokenization and encrypt sensitive data with Santum Bearer Token.**
* **Use Sanctum Connect for secure authentication.**
* **Implement role-based access control limit access based on user roles.**
* **Implement role-based access control limit access based on admin roles.**
* **Regularly audit and log access to sensitive data.**
{% endstep %}
{% endstepper %}

***

## Connecting Laravel and DotNet

{% stepper %}
{% step %}
### **RESTful APIs**

* Use Laravel and DotNet to expose RESTful APIs for mutual communication.
* Laravel handle Authentication For User, while DotNet manages backend operations Products.
* Laravel: Build APIs using Laravel's API Resource for secure token-based authentication.
* DotNet: Use ASP.NET Core Web API to create endpoints that can be consumed by Laravel.
{% endstep %}

{% step %}
### Mi**ddleware for Communication**

* **Use tools like Ngrok to expose local services for testing or build middleware in a microservices architecture to mediate data exchange.**
{% endstep %}

{% step %}
### D**atabase Sharing with PostgreSQL**

* **Both Laravel and DotNet can interact with the same PostgreSQL database in SupaBase Server.**
* **Use a schema-based approach to segregate responsibilities and prevent conflicts.**
{% endstep %}

{% step %}
### A**uthentication Bridging**

* **Use Sanctum to share authentication tokens securely between both systems.**
* **This ensures that users logged into Laravel can access DotNet-based features without re-authentication.**
{% endstep %}
{% endstepper %}

***

## Device and Platform Compatibility

{% stepper %}
{% step %}
### Cross-Platform AR Framework (MyWebAR)

* Use MyWebAR, a cloud-based, cross-platform AR SDK that works directly in web browsers (WebAR), eliminating the need for separate native apps. MyWebAR supports Android, iOS, and desktop devices through browsers.
* Leverage MyWebAR’s built-in device detection to check if the browser and device support AR features.
* Gracefully degrade AR features by showing fallback content (e.g., 3D Model “.glp”) if AR is not supported.
{% endstep %}

{% step %}
### Progressive Enhancement

* **Apply progressive enhancement to deliver the best possible experience depending on device and browser capabilities.**
* **Use lightweight AR models and compressed textures for mobile devices to reduce load time.**
* **Serve high-resolution models and richer AR interactions on high-end devices like flagship smartphones or tablets.**
* **Use MyWebAR’s optimization tools to balance performance and visual quality across platforms.**
{% endstep %}
{% endstepper %}

***

## Scalability and Infrastructure Challenges

{% stepper %}
{% step %}
### Cloud-Native Architecture

* **Use scalable cloud services like NGrok to handle variable traffic.**
* **Apply autoscaling, load balancing, and caching strategies.**
{% endstep %}

{% step %}
### Database Optimization

* **Optimize queries, indexes, and caching for both AR and regular data.**
* **Use Redis or Memcached for caching frequent queries.**
* **Periodically archive old data.**
{% endstep %}
{% endstepper %}

***

## Video overview

Got 2 minutes? Check out a video overview of our Project:

{% embed url="https://www.loom.com/embed/3bfa83acc9fd41b7b98b803ba9197d90" %}

***

{% content-ref url="../fundamentals/tools-used/" %}
[tools-used](../fundamentals/tools-used/)
{% endcontent-ref %}

{% content-ref url="../fundamentals/database-design.md" %}
[database-design.md](../fundamentals/database-design.md)
{% endcontent-ref %}

{% content-ref url="../user-guide/for-designers/" %}
[for-designers](../user-guide/for-designers/)
{% endcontent-ref %}

{% content-ref url="../developer-guide/for-developers/" %}
[for-developers](../developer-guide/for-developers/)
{% endcontent-ref %}

{% content-ref url="../refrences/refrences.md" %}
[refrences.md](../refrences/refrences.md)
{% endcontent-ref %}
